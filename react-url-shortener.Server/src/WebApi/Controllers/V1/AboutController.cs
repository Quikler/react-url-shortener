using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Services.About;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class AboutController(IAboutService aboutService) : BaseApiController
{
    /// <summary>
    /// Gets string repsresentation of url shortener algorithm
    /// </summary>
    /// <response code="200">Returns url shortener algorithm</response>
    [HttpGet(ApiRoutes.About.Get)]
    public async Task<IActionResult> GetAbout()
    {
        var result = await aboutService.GetAboutAsync();

        return result.Match(
            Ok,
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// Updates url shortener algorithm
    /// </summary>
    /// <response code="200">Returns url shortener algorithm</response>
    [Authorize(Roles = "Admin")]
    [HttpPut(ApiRoutes.About.Update)]
    public async Task<IActionResult> UpdateAbout([FromQuery] string newAbout)
    {
        var result = await aboutService.UpdateAboutAsync(newAbout);

        return result.Match(
            Ok,
            failure => failure.ToActionResult()
        );
    }
}
