namespace WebApi.DTOs.Identity;

public class AuthDto
{
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public required UserDto User { get; set; }
    public required string[] Roles { get; set; } = [];
}