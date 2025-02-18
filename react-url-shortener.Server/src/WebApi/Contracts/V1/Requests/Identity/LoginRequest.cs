namespace WebApi.Contracts.V1.Requests.Identity;

public record LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}