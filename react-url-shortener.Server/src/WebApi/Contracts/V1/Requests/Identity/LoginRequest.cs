namespace WebApi.Contracts.V1.Requests.Identity;

public class LoginRequest
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}