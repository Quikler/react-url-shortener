using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Contracts;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Url;
using WebApi.Hubs;
using WebApi.Hubs.Clients;
using WebApi.Mapping;
using WebApi.Services.UrlShortener;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

/// <summary>
/// Provides API endpoints for managing urls.
/// </summary>
[Produces("application/json")]
public class UrlsShortenerController(IUrlShortenerService urlShortenerService, IHubContext<UrlsHub, IUrlsClient> urlsHub) : BaseApiController
{
    /// <summary>
    /// Returns pagination of urls, otherwise error.
    /// </summary>
    /// <remarks>
    /// If invalid page number or page size are provided,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns pafination of urls.</response>
    /// <response code="400">Returns bad request error.</response>
    [HttpGet(ApiRoutes.Urls.GetAll)]
    [ProducesResponseType(typeof(PaginationResponse<UrlResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            return BadRequest("Invalid page number or page size");
        }

        var result = await urlShortenerService.GetAllAsync(pageNumber, pageSize);

        return result.Match(
            urlsDtoPagination => Ok(urlsDtoPagination.ToResponse(u => u.ToResponse(RootApiUrl))),
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// [Authenticated users] Returns info about url, otherwise error.
    /// </summary>
    /// <remarks>
    /// If url by id isn't found,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns info about url.</response>
    /// <response code="404">Returns not found error.</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet(ApiRoutes.Urls.GetInfo)]
    [ProducesResponseType(typeof(UrlInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInfo([FromRoute] Guid urlId)
    {
        var result = await urlShortenerService.GetInfoAsync(urlId);

        return result.Match(
            urlInfoDto => Ok(urlInfoDto.ToResponse(RootApiUrl)),
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// Redirects to original url by provided short code.
    /// </summary>
    /// <remarks>
    /// If url by short code is not found,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="301">Redirects to original url.</response>
    /// <response code="404">Returns not found error.</response>
    [HttpGet("/{shortCode}")]
    [ProducesResponseType(StatusCodes.Status301MovedPermanently)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RedirectToOriginal([FromRoute] string shortCode)
    {
        var result = await urlShortenerService.GetOriginalUrlByShortCodeAsync(shortCode);

        return result.Match(
            Redirect,
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// [Authenticated users] Creates shortened version of provided url.
    /// </summary>
    /// <remarks>
    /// If url already exist or error creating url,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="201">Returns created result.</response>
    /// <response code="400">Returns bad request error.</response>
    /// <response code="409">Returns conflict error.</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(ApiRoutes.Urls.Create)]
    [ProducesResponseType(typeof(UrlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromQuery][Url] string url)
    {
        if (!HttpContext.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await urlShortenerService.CreateShortenUrlAsync(url, userId);

        return await result.MatchAsync<IActionResult>(
            async urlDto =>
            {
                var response = urlDto.ToResponse(RootApiUrl);
                await urlsHub.Clients.All.CreateUrl(response);
                return CreatedAtAction(nameof(RedirectToOriginal), new { shortCode = urlDto.ShortCode }, response);
            },
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// [Authenticated users] Deletes url by provided url id.
    /// </summary>
    /// <remarks>
    /// If url doesn't exist or error deleting url or user is not authorized to perform delete,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="204">Returns no content.</response>
    /// <response code="400">Returns bad request error.</response>
    /// <response code="403">Returns forbidden error.</response>
    /// <response code="404">Returns not found error.</response>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete(ApiRoutes.Urls.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid urlId)
    {
        if (!HttpContext.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await urlShortenerService.DeleteUrlAsync(urlId, userId, HttpContext.GetUserRoles());

        return await result.MatchAsync<IActionResult>(
            async success =>
            {
                await urlsHub.Clients.All.DeleteUrl(urlId);
                return NoContent();
            },
            failure => failure.ToActionResult()
        );
    }
}
