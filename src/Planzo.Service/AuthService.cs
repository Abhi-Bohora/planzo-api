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
    private readonly IEmailService _emailService;

    public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
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
        var token = GenerateJwtToken(user);
        if (!result.Succeeded)
        {
            return new ResponseDto()
            {
                IsSuccess = false,
                Message = "Failed to create user",
                StatusCode = (int)HttpStatusCode.BadRequest,
            };
        }

        return new ResponseDto()
        {
            IsSuccess = true,
            Message = "User created successfully",
            StatusCode = (int)HttpStatusCode.Created,
            Token = token
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
            StatusCode = (int)HttpStatusCode.OK, 
            Token = token
        };
    }
    
    public async Task<ResponseDto> RequestPasswordResetAsync(PasswordResetRequestDto requestDto)
    {
        var user = await _userManager.FindByEmailAsync(requestDto.Email);
        if (user == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetToken);

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Password reset link sent to email",
            StatusCode = (int)HttpStatusCode.OK
        };
    }

    public async Task<ResponseDto> ConfirmPasswordResetAsync(PasswordResetConfirmDto resetDto)
    {
        if (resetDto.NewPassword != resetDto.ConfirmNewPassword)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Passwords do not match",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        var user = await _userManager.FindByEmailAsync(resetDto.Email);
        if (user == null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User not found",
                StatusCode = (int)HttpStatusCode.NotFound
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, resetDto.Token, resetDto.NewPassword);
        
        if (!result.Succeeded)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Password reset failed",
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Password reset successfully",
            StatusCode = (int)HttpStatusCode.OK
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