namespace WebApi.Contracts.V1.Responses.Identity;

public class UserResponse
{
    public required Guid Id { get; set; }
    public required string UserName { get; set; }
}