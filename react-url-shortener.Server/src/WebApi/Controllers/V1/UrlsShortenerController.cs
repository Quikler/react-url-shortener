using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Services.UrlShortener;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class UrlsShortenerController(IUrlShortenerService urlShortenerService) : ControllerBase
{
    [HttpGet(ApiRoutes.Urls.Get)]
    public async Task<IActionResult> Get([FromRoute] string shortCode)
    {
        var result = await urlShortenerService.GetOriginalUrlAsync(shortCode);

        return result.Match(
            Ok,
            failure => failure.ToActionResult()
        );
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost(ApiRoutes.Urls.Create)]
    public async Task<IActionResult> Create([FromQuery] string url)
    {
        var result = await urlShortenerService.CreateShortenUrlAsync(url);

        return result.Match(
            shortCode => CreatedAtAction(nameof(Get), new { shortCode }, shortCode),
            failure => failure.ToActionResult()
        );
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete(ApiRoutes.Urls.Delete)]
    public async Task<IActionResult> Delete([FromRoute] string shortCode)
    {
        var result = await urlShortenerService.DeleteUrlAsync(shortCode);

        return result.Match(
            success => NoContent(),
            failure => failure.ToActionResult()
        );
    }
}