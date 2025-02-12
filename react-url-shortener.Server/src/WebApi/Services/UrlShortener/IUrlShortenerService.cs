using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.UrlShortener;

public interface IUrlShortenerService
{
    Task<Result<string, FailureDto>> CreateShortenUrlAsync(string originalUrl);
    Task<Result<string, FailureDto>> GetOriginalUrlAsync(string shortenedUrl);
    Task<Result<bool, FailureDto>> DeleteUrlAsync(string shortenedUrl);
}