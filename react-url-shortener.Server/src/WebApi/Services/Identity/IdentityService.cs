using DAL;
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

namespace WebApi.Services.Identity;

public class IdentityService(AppDbContext dbContext,
    UserManager<UserEntity> userManager,
    TokenProvider tokenProvider,
    IOptions<JwtConfiguration> jwtConfiguration)
    : IIdentityService
{
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<Result<AuthDto, FailureDto>> SignupAsync(SignupDto signupDto)
    {
        if (await dbContext.Users.AnyAsync(u => u.UserName == signupDto.Username))
        {
            return FailureDto.Conflict("Username already exist.");
        }

        var user = new UserEntity
        {
            UserName = signupDto.Username,
        };

        var createResult = await userManager.CreateAsync(user, signupDto.Password);
        return createResult.Succeeded ? await GenerateAuthDtoForUserAsync(user) : FailureDto.BadRequest(createResult.Errors.Select(e => e.Description));
    }

    public async Task<Result<AuthDto, FailureDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username);

        if (user is null)
        {
            return FailureDto.Unauthorized("Invalid username or password.");
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        return isCorrectPassword ? await GenerateAuthDtoForUserAsync(user) : FailureDto.Unauthorized("Invalid username or password");
    }

    private async Task<AuthDto> GenerateAuthDtoForUserAsync(UserEntity user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime),
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();

        return CreateAuthDto(user, refreshToken.Token, tokenProvider.CreateToken(user, roles), [.. roles]);
    }

    private static AuthDto CreateAuthDto(UserEntity user, string refreshToken, string token, string[] roles) => new()
    {
        RefreshToken = refreshToken,
        Token = token,
        User = user.ToUserDto(),
        Roles = roles,
    };

    public async Task<Result<AuthDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return FailureDto.Unauthorized("Refresh token has expired.");
        }

        if (storedRefreshToken.User is null)
        {
            return FailureDto.Unauthorized("User not found.");
        }

        var newRefreshToken = tokenProvider.GenerateRefreshToken();
        storedRefreshToken.Token = newRefreshToken;
        storedRefreshToken.ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime);

        await dbContext.SaveChangesAsync();

        return await GenerateAuthDtoForUserAsync(storedRefreshToken.User);
    }

    public async Task<Result<AuthDto, FailureDto>> MeAsync(string refreshToken)
    {
        var storedRefreshToken = await dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == refreshToken);

        if (storedRefreshToken is null || storedRefreshToken.ExpiryDate < DateTime.UtcNow)
        {
            return FailureDto.Unauthorized("Refresh token has expired.");
        }

        if (storedRefreshToken.User is null)
        {
            return FailureDto.Unauthorized("User not found.");
        }

        return await GenerateAuthDtoForUserAsync(storedRefreshToken.User);
    }
}