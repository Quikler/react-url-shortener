using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Services.UrlShortener;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class UrlsShortenerController(IUrlShortenerService urlShortenerService) : ControllerBase
{
    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToOriginal([FromRoute] string shortCode)
    {
        var result = await urlShortenerService.GetOriginalUrlAsync(shortCode);

        return result.Match(
            Redirect,
            failure => failure.ToActionResult()
        );
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(ApiRoutes.Urls.Create)]
    public async Task<IActionResult> Create([FromQuery] [Url] string url)
    {
        if (!HttpContext.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await urlShortenerService.CreateShortenUrlAsync(url, userId);

        return result.Match(
            shortCode => CreatedAtAction(nameof(RedirectToOriginal), new { shortCode }, shortCode),
            failure => failure.ToActionResult()
        );
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete(ApiRoutes.Urls.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string shortCode)
    {
        if (!HttpContext.TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await urlShortenerService.DeleteUrlAsync(shortCode, userId);

        return result.Match(
            success => NoContent(),
            failure => failure.ToActionResult()
        );
    }
}