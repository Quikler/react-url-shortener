namespace WebApi.Contracts.V1.Responses.Identity;

public record UserResponse
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
}