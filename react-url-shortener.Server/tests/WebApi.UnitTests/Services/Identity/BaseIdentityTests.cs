using DAL;
using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using WebApi.Configurations;
using WebApi.Providers;
using WebApi.Repositories.RefreshToken;
using WebApi.Services.Identity;
using WebApi.Services.Unit;

namespace WebApi.UnitTests.Services.Identity;

public class BaseIdentityTests : BaseUnitTest
{
    protected Mock<UserManager<UserEntity>> UserManagerMock { get; }
    protected Mock<RefreshTokenRepository> RefreshTokenRepositoryMock { get; }
    protected Mock<UnitOfWork> UnitOfWorkMock { get; }

    protected TokenProvider TokenProvider { get; }
    protected JwtConfiguration JwtConfiguration { get; }

    protected IdentityService IdentityService { get; }

    public BaseIdentityTests()
    {
        var dbContext = new Mock<AppDbContext>();

        var userStoreMock = new Mock<IUserStore<UserEntity>>();
        UserManagerMock = new Mock<UserManager<UserEntity>>(userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        RefreshTokenRepositoryMock = new Mock<RefreshTokenRepository>(dbContext.Object);

        UnitOfWorkMock = new Mock<UnitOfWork>(dbContext.Object);

        JwtConfiguration = new JwtConfiguration
        {
            SecretKey = "f)6c4[w(zm:[gU?WQd}J)8*)X:?d8:&P",
            ValidIssuer = "https://localhost:7207",
            ValidAudience = "https://localhost:7207",
            TokenLifetime = TimeSpan.FromSeconds(45),
            RefreshTokenLifetime = TimeSpan.FromDays(180),
        };

        var jwtConfigurationOptions = Options.Create(JwtConfiguration);

        TokenProvider = new TokenProvider(jwtConfigurationOptions);

        IdentityService = new IdentityService(UserManagerMock.Object, RefreshTokenRepositoryMock.Object, UnitOfWorkMock.Object, TokenProvider, jwtConfigurationOptions);
    }
}