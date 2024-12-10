using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Planzo.Data.Dtos.Auth;
using Planzo.Service.Interfaces;

namespace Planzo.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
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

        var result = await authService.SignUpAsync(signUpDto); 
        return result.IsSuccess 
            ? StatusCode(result.StatusCode, result) 
            : BadRequest(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return result.IsSuccess 
            ? StatusCode(result.StatusCode, result) 
            : BadRequest(result);
    }
    
    [HttpPost("request-password-reset")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto requestDto)
    {
        var result = await authService.RequestPasswordResetAsync(requestDto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("confirm-password-reset")]
    public async Task<IActionResult> ConfirmPasswordReset([FromBody] PasswordResetConfirmDto resetDto)
    {
        var result = await authService.ConfirmPasswordResetAsync(resetDto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}