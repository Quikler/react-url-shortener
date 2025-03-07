using WebApi.Contracts.V1.Responses.Url;

namespace WebApi.Hubs.Clients;

public interface IUrlsClient
{
    Task CreateUrl(UrlResponse url);
    Task DeleteUrl(Guid urlId);
}