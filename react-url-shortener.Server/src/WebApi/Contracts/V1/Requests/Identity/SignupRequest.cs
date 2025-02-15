using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.V1.Requests.Identity;

public class SignupRequest
{
    public required string Username { get; set; }
    
    [Compare(nameof(ConfirmPassword), ErrorMessage = "Passwords do not match.")]
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}