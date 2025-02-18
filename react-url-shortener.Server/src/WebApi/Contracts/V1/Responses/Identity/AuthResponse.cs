namespace WebApi.Contracts.V1.Responses.Identity;

public record AuthResponse
{
    public required string Token { get; set; }
    public required UserResponse User { get; set; }
    public required IEnumerable<string> Roles { get; set; }
}