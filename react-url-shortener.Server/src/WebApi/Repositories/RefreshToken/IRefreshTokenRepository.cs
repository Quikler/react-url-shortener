using DAL.Entities;

namespace WebApi.Repositories.RefreshToken;

public interface IRefreshTokenRepository
{
    Task<RefreshTokenEntity?> GetRefreshTokenWithUserAsync(string refreshToken, bool tracking = false);
    void AddRefreshToken(RefreshTokenEntity refreshToken);
}