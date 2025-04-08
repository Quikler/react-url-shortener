using DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Common;
using WebApi.Configurations;
using WebApi.DTOs;
using WebApi.DTOs.Identity;
using WebApi.Mapping;
using WebApi.Providers;
using WebApi.Repositories.RefreshToken;
using WebApi.Services.Unit;

namespace WebApi.Services.Identity;

/// <summary>
/// Provides functionality for managing user authentication, identification and registration
/// </summary>
public class IdentityService(UserManager<UserEntity> userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration)
    : IIdentityService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    /// <summary>
    /// Asynchronously signs up into account and returns JWT, Refresh token and info new logged in user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing JWT, Refresh token and info about user,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If user already exist a <see cref="FailureDto.Conflict(string)"/> with an conflict message is returned.
    /// If unable to create user a <see cref="FailureDto.BadRequest(string)"/> with an bad request message is returned.
    /// </remarks>
    /// <param name="signupDto">Represents sign up dto.</param>
    public async Task<Result<AuthDto, FailureDto>> SignupAsync(SignupDto signupDto)
    {
        if (await userManager.Users.AnyAsync(u => u.UserName == signupDto.Username))
        {
            return FailureDto.Conflict(IdentityServiceMessages.USERNAME_ALREADY_EXIST);
        }

        var user = new UserEntity
        {
            UserName = signupDto.Username,
        };

        var createResult = await userManager.CreateAsync(user, signupDto.Password);
        return createResult.Succeeded ? await GenerateAuthDtoForUserAsync(user) : FailureDto.BadRequest(createResult.Errors.Select(e => e.Description));
    }

    /// <summary>
    /// Asynchronously logins into account and returns JWT, Refresh token and info about logged in user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing JWT, Refresh token and info about user,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If user is not found or if password is invalid
    /// a <see cref="FailureDto.Unauthorized(string)"/> with an unauthorized message is returned.
    /// </remarks>
    /// <param name="loginDto">Represents login dto.</param>
    public async Task<Result<AuthDto, FailureDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.Username);
        if (user is null)
        {
            return FailureDto.Unauthorized(IdentityServiceMessages.INVALID_USERNAME_OR_PASSWORD);
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        return isCorrectPassword ? await GenerateAuthDtoForUserAsync(user) : FailureDto.Unauthorized(IdentityServiceMessages.INVALID_USERNAME_OR_PASSWORD);
    }

    /// <summary>
    /// Asynchronously refreshes the token and returns JWT, Refresh token and info about logged in user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing JWT, Refresh token and info about user,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If refresh token doesn't exist or expired or user doesn't exist
    /// a <see cref="FailureDto.Unauthorized(string)"/> with an unauthorized message is returned.
    /// </remarks>
    /// <param name="refreshToken">Represents token field of refresh token entity.</param>
    public async Task<Result<AuthDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await refreshTokenRepository.GetRefreshTokenWithUserAsync(refreshToken, true);
        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return FailureDto.Unauthorized(IdentityServiceMessages.REFRESH_TOKEN_EXPIRED);
        }

        if (storedRefreshToken.User is null)
        {
            return FailureDto.Unauthorized(IdentityServiceMessages.USER_NOT_FOUND);
        }

        var newRefreshToken = tokenProvider.GenerateRefreshToken();
        storedRefreshToken.Token = newRefreshToken;
        storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime);

        await unitOfWork.SaveChangesAsync();

        var roles = await userManager.GetRolesAsync(storedRefreshToken.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthDto(storedRefreshToken.User, newRefreshToken, token, [.. roles]);
    }

    /// <summary>
    /// Asynchronously returns JWT, Refresh token and info about logged in user.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing JWT, Refresh token and info about user,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If refresh token doesn't exist or expired or user doesn't exist
    /// a <see cref="FailureDto.Unauthorized(string)"/> with an unauthorized message is returned.
    /// </remarks>
    /// <param name="refreshToken">Represents token field of refresh token entity.</param>
    public async Task<Result<AuthDto, FailureDto>> MeAsync(string refreshToken)
    {
        var storedRefreshToken = await refreshTokenRepository.GetRefreshTokenWithUserAsync(refreshToken);
        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return FailureDto.Unauthorized(IdentityServiceMessages.REFRESH_TOKEN_EXPIRED);
        }

        if (storedRefreshToken.User is null)
        {
            return FailureDto.Unauthorized(IdentityServiceMessages.USER_NOT_FOUND);
        }

        var roles = await userManager.GetRolesAsync(storedRefreshToken!.User);
        var token = tokenProvider.CreateToken(storedRefreshToken.User, roles);

        return CreateAuthDto(storedRefreshToken.User, storedRefreshToken.Token, token, [.. roles]);
    }

    private async Task<AuthDto> GenerateAuthDtoForUserAsync(UserEntity user)
    {
        IList<string> roles = await userManager.GetRolesAsync(user);

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            User = user,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime),
        };

        refreshTokenRepository.AddRefreshToken(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return CreateAuthDto(user, refreshToken.Token, tokenProvider.CreateToken(user, roles), [.. roles]);
    }

    private static AuthDto CreateAuthDto(UserEntity user, string refreshToken, string token, string[] roles) => new()
    {
        RefreshToken = refreshToken,
        Token = token,
        User = user.ToUserDto(),
        Roles = roles,
    };
}
