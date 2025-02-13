namespace WebApi.Services.UrlShortener;

public interface IUrlShortenerAuthorizationService
{
    Task<bool> IsUserOwnsShortenedUrlAsync(Guid userId, string originalUrl);
}