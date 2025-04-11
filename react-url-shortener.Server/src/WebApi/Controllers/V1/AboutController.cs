using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.About;
using WebApi.Contracts.V1.Responses;
using WebApi.Services.About;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

/// <summary>
/// Provides API endpoints to interact with about-alg.txt file.
/// </summary>
[Produces("application/json")]
public class AboutController(IAboutService aboutService) : BaseApiController
{
    /// <summary>
    /// Gets string repsresentation of url shortener algorithm
    /// </summary>
    /// <response code="200">Returns url shortener algorithm.</response>
    /// <response code="400">Returns bad request message.</response>
    [HttpGet(ApiRoutes.About.Get)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAbout()
    {
        var result = await aboutService.GetAboutAsync();

        return result.Match(
            Ok,
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// [Admin] Updates url shortener algorithm
    /// </summary>
    /// <response code="200">Returns url shortener algorithm</response>
    [Authorize(Roles = "Admin")]
    [HttpPut(ApiRoutes.About.Update)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateAbout([FromBody] UpdateAboutRequest request)
    {
        var result = await aboutService.UpdateAboutAsync(request.AboutText);

        return result.Match(
            Ok,
            failure => failure.ToActionResult()
        );
    }
}
