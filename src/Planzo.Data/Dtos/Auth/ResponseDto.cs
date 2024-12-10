namespace Planzo.Data.Dtos.Auth;

public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public string? Token { get; set; } 
}