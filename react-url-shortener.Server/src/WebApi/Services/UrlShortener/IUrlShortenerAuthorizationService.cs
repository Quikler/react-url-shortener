namespace WebApi.Services.UrlShortener;

public interface IUrlShortenerAuthorizationService
{
    Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId);
    Task<bool> IsUserAuthorizedAsync(Guid userId, Guid urlId, string[] roles);
}