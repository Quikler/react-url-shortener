namespace WebApi.DTOs.Identity;

public class LoginDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}