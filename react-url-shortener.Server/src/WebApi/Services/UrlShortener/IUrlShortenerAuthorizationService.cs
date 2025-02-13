namespace WebApi.Services.UrlShortener;

public interface IUrlShortenerAuthorizationService
{
    Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId);
}