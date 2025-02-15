using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;
using WebApi.Contracts.V1.Requests.Identity;
using WebApi.Mapping;
using WebApi.Services.Identity;
using WebApi.Utils.Extensions;

namespace WebApi.Controllers.V1;

public class IdentityController(IIdentityService identityService) : ControllerBase
{
    [HttpPost(ApiRoutes.Identity.Login)]
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

    [HttpPost(ApiRoutes.Identity.Signup)]
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

    [HttpPost(ApiRoutes.Identity.Logout)]
    public async Task<IActionResult> Logout()
    {
        //HttpContext.Response.Cookies.Delete("refreshToken");
        await HttpContext.SignOutAsync();
        return NoContent();
    }

    [HttpPost(ApiRoutes.Identity.Refresh)]
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

    [HttpGet(ApiRoutes.Identity.Me)]
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