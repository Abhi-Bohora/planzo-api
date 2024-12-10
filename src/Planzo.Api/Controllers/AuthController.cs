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
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(ResponseDto), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var result = await authService.LoginAsync(loginDto);
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Message);
    }
}