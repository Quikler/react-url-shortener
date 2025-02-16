using WebApi.DTOs.Identity;

namespace WebApi.DTOs.Url;

public class UrlInfoDto
{
    public required Guid Id { get; set; }

    public required string UrlOriginal { get; set; }
    public required string ShortCode { get; set; }
    public required DateTime CreatedAt { get; set; }

    public required Guid UserId { get; set; }

    public required UserDto User { get; set; }
}