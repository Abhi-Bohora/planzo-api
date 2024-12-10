using Planzo.Data.Dtos.Auth;

namespace Planzo.Service.Interfaces;

public interface IAuthService
{
    public Task<ResponseDto> SignUpAsync(SignUpDto signUpDto);
    public Task<ResponseDto> LoginAsync(LoginDto loginDto); 
}