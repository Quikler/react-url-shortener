namespace WebApi.DTOs.Identity;

public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}