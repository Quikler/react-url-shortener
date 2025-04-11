using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Contracts.V1.Responses;
using WebApi.Contracts.V1.Responses.Identity;
using WebApi.Mapping;
using WebApi.Services.Identity;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

/// <summary>
/// Provides API endpoints for managing user authentication, identification and authorization.
/// </summary>
[Produces("application/json")]
public class IdentityController(IIdentityService identityService) : ControllerBase
{
    /// <summary>
    /// Logs in user and returns user information, otherwise error is returned.
    /// </summary>
    /// <remarks>
    /// If incorrect credentials are provided,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns user information.</response>
    /// <response code="401">Returns unauthorized error.</response>
    [HttpPost(ApiRoutes.Identity.Login)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await identityService.LoginAsync(loginRequest.ToDto());

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// Signs up user and returns user information, otherwise error is returned.
    /// </summary>
    /// <remarks>
    /// If a user with the same username already exists or an error occurs,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns user information.</response>
    /// <response code="400">Returns bad request error.</response>
    /// <response code="409">Returns conflict error.</response>
    [HttpPost(ApiRoutes.Identity.Signup)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Signup([FromBody] SignupRequest signupRequest)
    {
        var result = await identityService.SignupAsync(signupRequest.ToDto());

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// Deletes the refresh token cookie to logout the user.
    /// </summary>
    /// <response code="204">Returns no content.</response>
    [HttpPost(ApiRoutes.Identity.Logout)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("refreshToken");
        return NoContent();
    }

    /// <summary>
    /// Refreshes jwt token and returns user information, otherwise error is returned.
    /// </summary>
    /// <remarks>
    /// If refresh token is not presented in cookie under "refreshToken" key or
    /// refresh token/user is not found,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns user information.</response>
    /// <response code="401">Returns unauthorized error.</response>
    [HttpPost(ApiRoutes.Identity.Refresh)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Refresh()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();

        var result = await identityService.RefreshTokenAsync(refreshToken);

        return result.Match(
            authSuccessDto => 
            {
                HttpContext.SetHttpOnlyRefreshToken(authSuccessDto.RefreshToken);
                return Ok(authSuccessDto.ToResponse());
            },
            failure => failure.ToActionResult()
        );
    }

    /// <summary>
    /// Retrieves the user associated with current refresh token cookie, otherwise error is returned.
    /// </summary>
    /// <remarks>
    /// If refresh token not presented in cookie under "refreshToken" key or
    /// refresh token/user is not found,
    /// a <see cref="FailureResponse"/> with an error message is returned.
    /// </remarks>
    /// <response code="200">Returns user information.</response>
    /// <response code="401">Returns unauthorized error.</response>
    [HttpGet(ApiRoutes.Identity.Me)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(FailureResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken)) return Unauthorized();

        var result = await identityService.MeAsync(refreshToken);

        return result.Match(
            authSuccessDto => Ok(authSuccessDto.ToResponse()),
            failure => failure.ToActionResult()
        );
    }
}
