namespace DAL.Entities;

public class UrlEntity : BaseEntity
{
    public required string UrlOriginal { get; set; }
    public required string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}