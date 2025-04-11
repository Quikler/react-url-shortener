using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Url;

namespace WebApi.Services.UrlShortener;

/// <summary>
/// Provides functionality for creating, reading, updating, deleting (CRUD) urls.
/// </summary>
public interface IUrlShortenerService
{
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
    Task<Result<PaginationDto<UrlDto>, FailureDto>> GetAllAsync(int pageNumber, int pageSize);

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
    Task<Result<string, FailureDto>> GetOriginalUrlByShortCodeAsync(string shortCode);

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
    Task<Result<UrlDto, FailureDto>> CreateShortenUrlAsync(string originalUrl, Guid userId);

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
    Task<Result<bool, FailureDto>> DeleteUrlAsync(Guid urlId, Guid userId, string[] userRoles);

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
    Task<Result<UrlInfoDto, FailureDto>> GetInfoAsync(Guid urlId);
}
