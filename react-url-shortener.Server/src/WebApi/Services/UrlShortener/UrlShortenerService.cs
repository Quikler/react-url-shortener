using System.Security.Cryptography;
using System.Text;
using DAL.Entities;
using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Url;
using WebApi.Mapping;
using WebApi.Repositories.Url;
using WebApi.Services.Unit;

namespace WebApi.Services.UrlShortener;

public class UrlShortenerService(IUnitOfWork unitOfWork, IUrlRepository urlRepository) : IUrlShortenerService
{
    private readonly IUrlRepository _urlRepository = urlRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<PaginationDto<UrlDto>, FailureDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _urlRepository.GetAllUrlDtoAsync(pageNumber, pageSize);
    }

    public async Task<Result<string, FailureDto>> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        var urlOriginal = await _urlRepository.GetOriginalUrlByShortCodeAsync(shortCode);
        return urlOriginal is null ? FailureDto.NotFound(UrlShortenerServiceMessages.URL_NOT_FOUND) : urlOriginal;
    }

    public async Task<Result<UrlDto, FailureDto>> CreateShortenUrlAsync(string originalUrl, Guid userId)
    {
        if (await _urlRepository.IsUrlOriginalExistAsync(originalUrl))
        {
            return FailureDto.Conflict(UrlShortenerServiceMessages.URL_ALREADY_EXIST);
        }

        var shortCode = GenerateShortCode();
        var url = new UrlEntity
        {
            UrlOriginal = originalUrl,
            ShortCode = shortCode,
            UserId = userId,
        };

        _urlRepository.AddUrl(url);
        int rows = await _unitOfWork.SaveChangesAsync();

        return rows == 0 ? FailureDto.BadRequest(UrlShortenerServiceMessages.CANNOT_CREATE_URL) : url.ToUrlDto();
    }

    public async Task<Result<bool, FailureDto>> DeleteUrlAsync(Guid urlId, Guid userId, string[] userRoles)
    {
        if (!await _urlRepository.IsUrlByIdExistAsync(urlId))
        {
            return FailureDto.NotFound(UrlShortenerServiceMessages.URL_NOT_FOUND);
        }

        if (!await _urlRepository.IsUserOwnerOrAdminAsync(userId, urlId, userRoles))
        {
            return FailureDto.Forbidden(UrlShortenerServiceMessages.USER_DOESNT_AUTHORIZED_TO_URL);
        }

        int rows = await _urlRepository.DeleteUrlAsync(urlId);
        return rows == 0 ? FailureDto.BadRequest(UrlShortenerServiceMessages.CANNOT_DELETE_URL) : true;
    }

    public async Task<Result<UrlInfoDto, FailureDto>> GetInfoAsync(Guid urlId)
    {
        var url = await _urlRepository.GetUrlInfoDtoAsync(urlId);
        return url is null ? FailureDto.NotFound(UrlShortenerServiceMessages.URL_NOT_FOUND) : url;
    }

    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private static string GenerateShortCode(int length = 6)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(length);
        var shortCode = new StringBuilder(length);

        foreach (var b in randomBytes)
        {
            shortCode.Append(Base62Chars[b % Base62Chars.Length]);
        }

        return shortCode.ToString();
    }
}
