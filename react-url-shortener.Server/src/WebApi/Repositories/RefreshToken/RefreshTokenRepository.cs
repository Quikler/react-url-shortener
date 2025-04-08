using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories.RefreshToken;

/// <summary>
/// Provides communication with RefreshToken set of db context.
/// </summary>
public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    /// <summary>
    /// Begins tracking the given refresh token entity in the <see cref="EntityState.Added"/> state.
    /// </summary>
    /// <param name="refreshToken">Represents refresh token entity in database.</param>
    public virtual void AddRefreshToken(RefreshTokenEntity refreshToken) => _dbContext.RefreshTokens.Add(refreshToken);

    /// <summary>
    /// Retrieves refresh token including User reference by refresh token.
    /// </summary>
    /// <param name="refreshToken">Represents token field of refresh token entity.</param>
    /// <param name="tracking">Represents boolean value whether to track antity with or not.</param>
    public virtual Task<RefreshTokenEntity?> GetRefreshTokenWithUserAsync(string refreshToken, bool tracking = false)
    {
        IQueryable<RefreshTokenEntity> query = _dbContext.RefreshTokens.Include(r => r.User);

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(r => r.Token == refreshToken);
    }
}
