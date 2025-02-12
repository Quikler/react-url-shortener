namespace WebApi.Contracts.V1.Responses.Identity;

public class AuthResponse
{
    public required string Token { get; set; }
    public required UserResponse User { get; set; }
}