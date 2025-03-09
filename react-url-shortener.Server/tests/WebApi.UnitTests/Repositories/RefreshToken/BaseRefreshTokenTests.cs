using DAL;
using Moq;
using WebApi.Repositories.RefreshToken;

namespace WebApi.UnitTests.Repositories.RefreshToken;

public class BaseRefreshTokenTests : BaseUnitTest
{
    protected Mock<AppDbContext> DbContextMock { get; }

    protected RefreshTokenRepository RefreshTokenRepository { get; }

    public BaseRefreshTokenTests()
    {
        DbContextMock = new Mock<AppDbContext>();

        RefreshTokenRepository = new RefreshTokenRepository(DbContextMock.Object);
    }
}