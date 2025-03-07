using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApi.Contracts;
using WebApi.Hubs;
using WebApi.Hubs.Clients;
using WebApi.Mapping;
using WebApi.Services.UrlShortener;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class UrlsShortenerController(IUrlShortenerService urlShortenerService, IHubContext<UrlsHub, IUrlsClient> urlsHub) : BaseApiController
{
    [HttpGet(ApiRoutes.Urls.GetAll)]
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet(ApiRoutes.Urls.GetInfo)]
    public async Task<IActionResult> GetInfo([FromRoute] Guid urlId)
    {
        var result = await urlShortenerService.GetInfoAsync(urlId);

        return result.Match(
            urlInfoDto => Ok(urlInfoDto.ToResponse(RootApiUrl)),
            failure => failure.ToActionResult()
        );
    }

    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal([FromRoute] string shortCode)
    {
        var result = await urlShortenerService.GetOriginalUrlByShortCodeAsync(shortCode);

        return result.Match(
            Redirect,
            failure => failure.ToActionResult()
        );
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(ApiRoutes.Urls.Create)]
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

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete(ApiRoutes.Urls.Delete)]
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