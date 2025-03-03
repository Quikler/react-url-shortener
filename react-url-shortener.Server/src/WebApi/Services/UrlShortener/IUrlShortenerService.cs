using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Url;

namespace WebApi.Services.UrlShortener;

public interface IUrlShortenerService
{
    Task<Result<PaginationDto<UrlDto>, FailureDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<Result<UrlDto, FailureDto>> CreateShortenUrlAsync(string originalUrl, Guid userId);
    Task<Result<string, FailureDto>> GetOriginalUrlByShortCodeAsync(string shortCode);
    Task<Result<bool, FailureDto>> DeleteUrlAsync(Guid urlId, Guid userId, string[] userRoles);
    Task<Result<UrlInfoDto, FailureDto>> GetInfoAsync(Guid urlId);
}