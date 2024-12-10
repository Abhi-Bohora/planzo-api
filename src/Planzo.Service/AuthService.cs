using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Planzo.Data.Configurations;
using Planzo.Data.Dtos.Auth;
using Planzo.Service.Interfaces;

namespace Planzo.Service;

public class AuthService:IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    
    public async Task<ResponseDto> SignUpAsync(SignUpDto signUpDto)
    {
        var user = new IdentityUser {UserName = signUpDto.Email, Email = signUpDto.Email};
        var checkUser = await _userManager.FindByEmailAsync(signUpDto.Email);
        if (checkUser != null)
        {
            return new ResponseDto {
                IsSuccess = false, 
                Message = "Email already exists",
                StatusCode = (int)HttpStatusCode.BadRequest};
        }
        var result = await _userManager.CreateAsync(user, signUpDto.Password);
        if (!result.Succeeded)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                Message = "Failed to create user",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ResponseDto()
        {
            IsSuccess = true,
            Message = "User created successfully",
            StatusCode = (int)HttpStatusCode.Created
        };
    }

    public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginDto.Email,
            loginDto.Password, false, false);

        if (!result.Succeeded)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                Message = "Invalid login attempt.",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        var token = GenerateJwtToken(user);
        return new ResponseDto()
        {
            IsSuccess = true,
            Message = "Login successful.",
            Data = new TokenResponse { Token = token }
        };
    }
    
    private string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(PlanzoConfig.JwtSettings?.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: PlanzoConfig.JwtSettings?.Issuer,
            audience: PlanzoConfig.JwtSettings?.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}