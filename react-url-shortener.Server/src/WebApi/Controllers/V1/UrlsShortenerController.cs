using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Mapping;
using WebApi.Services.UrlShortener;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class UrlsShortenerController(IUrlShortenerService urlShortenerService) : BaseApiController
{
    [HttpGet(ApiRoutes.Urls.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
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

        return result.Match(
            urlDto => CreatedAtAction(nameof(RedirectToOriginal), new { shortCode = urlDto.ShortCode }, urlDto.ToResponse(RootApiUrl)),
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

        return result.Match(
            success => NoContent(),
            failure => failure.ToActionResult()
        );
    }
}