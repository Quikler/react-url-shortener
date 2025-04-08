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

/// <summary>
/// Provides functionality for creating, reading, updating, deleting (CRUD) urls.
/// </summary>
public class UrlShortenerService(IUnitOfWork unitOfWork, IUrlRepository urlRepository) : IUrlShortenerService
{
    private readonly IUrlRepository _urlRepository = urlRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Asynchronously retrieves urls pagination.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing pagination dto of urls,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If something gost wrong with database query a <see cref="FailureDto.BadRequest(string)"/> with a bad request message is returned.
    /// </remarks>
    /// <param name="pageNumber">Represents page number from where to retrieve url dtos.</param>
    /// <param name="pageSize">Represents page size of pagination.</param>
    public async Task<Result<PaginationDto<UrlDto>, FailureDto>> GetAllAsync(int pageNumber, int pageSize)
    {
        try
        {
            return await _urlRepository.GetAllUrlDtoAsync(pageNumber, pageSize);
        }
        catch (System.Exception)
        {
            return FailureDto.BadRequest(UrlShortenerServiceMessages.CANNOT_GET_URLS);
        }
    }

    /// <summary>
    /// Asynchronously retrieves url by short code.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing the string of original url,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If url not found a <see cref="FailureDto.NotFound(string)"/> with a not found message is returned.
    /// </remarks>
    /// <param name="shortCode">Represents short code of original url.</param>
    public async Task<Result<string, FailureDto>> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        var urlOriginal = await _urlRepository.GetOriginalUrlByShortCodeAsync(shortCode);
        return urlOriginal is null ? FailureDto.NotFound(UrlShortenerServiceMessages.URL_NOT_FOUND) : urlOriginal;
    }

    /// <summary>
    /// Asynchronously creates shorten url from original url.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing the url dto,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If url already exists a <see cref="FailureDto.Conflict(string)"/> with a conflict message is returned.
    /// If something goes wrong a <see cref="FailureDto.BadRequest(string)"/> with a bad request message is returned.
    /// </remarks>
    /// <param name="originalUrl">Represents original url from which shortened version will be created.</param>
    /// <param name="userId">Represents user id who creates url.</param>
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

    /// <summary>
    /// Asynchronously deletes shorten url by url id.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing the boolean result,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If url not found a <see cref="FailureDto.NotFound(string)"/> with a not found message is returned.
    /// If user is not owner or admin <see cref="FailureDto.Forbidden(string)"/> with a forbidden message is returned.
    /// If delete wasn't performed a <see cref="FailureDto.BadRequest(string)"/> with a bad request message is returned.
    /// </remarks>
    /// <param name="urlId">Represents unique identifier of url.</param>
    /// <param name="userId">Represents unique identifier of user who wants to delete url.</param>
    /// <param name="userRoles">Represents an array of roles which user who wants to delete url has.</param>
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

    /// <summary>
    /// Asynchronously gets info about url by url id.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing url info dto,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If url not found a <see cref="FailureDto.NotFound(string)"/> with a not found message is returned.
    /// </remarks>
    /// <param name="urlId">Represents unique identifier of url.</param>
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
