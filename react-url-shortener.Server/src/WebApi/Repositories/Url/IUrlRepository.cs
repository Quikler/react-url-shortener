using DAL.Entities;
using WebApi.DTOs;
using WebApi.DTOs.Url;

namespace WebApi.Repositories.Url;

public interface IUrlRepository
{
    Task<PaginationDto<UrlDto>> GetAllUrlDtoAsync(int pageNumber, int pageSize);
    Task AddUrlAsync(UrlEntity urlEntity);
    void AddUrl(UrlEntity urlEntity);
    Task<string?> GetOriginalUrlByShortCodeAsync(string shortCode);
    Task<UrlInfoDto?> GetUrlInfoDtoAsync(Guid urlId);
    Task<bool> IsUrlOriginalExistAsync(string originalUrl);
    Task<bool> IsUrlByIdExistAsync(Guid urlId);
    Task<int> DeleteUrlAsync(Guid urlId);
    Task<bool> IsUserOwnsUrlAsync(Guid userId, Guid urlId);
    Task<bool> IsUserOwnerOrAdminAsync(Guid userId, Guid urlId, string[] roles);
}