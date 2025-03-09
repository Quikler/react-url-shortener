using DAL;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using WebApi.Repositories.Url;
using WebApi.Services.Unit;
using WebApi.Services.UrlShortener;

namespace WebApi.UnitTests.Services.UrlShortener;

public class BaseUrlShortenerTests : BaseUnitTest
{
    protected Mock<UnitOfWork> UnitOfWorkMock { get; }
    protected Mock<UrlRepository> UrlRepositoryMock { get; }
    
    protected UrlShortenerService UrlShortenerService { get; }

    public BaseUrlShortenerTests()
    {
        var dbContext = new Mock<AppDbContext>();
        var memoryCache = new Mock<IMemoryCache>();

        UnitOfWorkMock = new Mock<UnitOfWork>(dbContext.Object);
        UrlRepositoryMock = new Mock<UrlRepository>(dbContext.Object, memoryCache.Object);

        UrlShortenerService = new UrlShortenerService(UnitOfWorkMock.Object, UrlRepositoryMock.Object);
    }
}