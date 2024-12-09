using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Planzo.Data.Dtos.Auth;

namespace Planzo.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IConfiguration _configuration;
    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;  
    }

    [HttpPost("signup")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
    {
        if (signUpDto.Password != signUpDto.ConfirmPassword)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Passwords do not match",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }
        
        var user = new IdentityUser {UserName = signUpDto.Email, Email = signUpDto.Email};
        var result = await _userManager.CreateAsync(user, signUpDto.Password);
        if (!result.Succeeded)
        {
            return BadRequest( new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to create user",
                StatusCode = (int)HttpStatusCode.BadRequest
            });
        }

        return CreatedAtAction(nameof(SignUp), new ResponseDto
        {
            IsSuccess = true,
            Message = "User created successfully",
            StatusCode = (int)HttpStatusCode.Created
        });
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await _signInManager.PasswordSignInAsync(loginDto.Email,
            loginDto.Password, false, false);

        if (!result.Succeeded)
        {
            return BadRequest(new ResponseDto
            {
                IsSuccess = false,
                Message = "Invalid login attempt.",
                StatusCode = StatusCodes.Status400BadRequest
            });
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        var token = GenerateJwtToken(user);

        return Ok(new ResponseDto
        {
            IsSuccess = true,
            Message = "Login successful.",
            Data = new TokenResponse { Token = token }
        });
    }

    public string GenerateJwtToken(IdentityUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}