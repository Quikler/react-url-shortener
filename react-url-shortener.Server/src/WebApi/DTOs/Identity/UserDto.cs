namespace WebApi.DTOs.Identity;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
}