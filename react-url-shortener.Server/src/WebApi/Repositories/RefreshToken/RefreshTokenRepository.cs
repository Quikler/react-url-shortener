using DAL;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Repositories.RefreshToken;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public virtual void AddRefreshToken(RefreshTokenEntity refreshToken) => _dbContext.RefreshTokens.Add(refreshToken);

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