namespace WebApi.DTOs.Identity;

public class SignupDto
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
}