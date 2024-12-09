namespace Planzo.Data.Dtos.Auth;
public interface IResponseData { }

public class TokenResponse : IResponseData
{
    public string Token { get; set; }
}

public class ResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public IResponseData? Data { get; set; }
}