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

/// <inheritdoc/>
public class IdentityService(UserManager<UserEntity> userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration)
    : IIdentityService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
