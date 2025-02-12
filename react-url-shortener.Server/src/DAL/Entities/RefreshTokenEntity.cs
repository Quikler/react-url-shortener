namespace DAL.Entities;

public class RefreshTokenEntity : BaseEntity
{
    public required string Token { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public required DateTime ExpiryDate { get; set; }

    public required Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}