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
        if (await dbContext.Users.AnyAsync(u => u.UserName == signupDto.UserName)) return FailureDto.Conflict("Username already exist.");

        var user = new UserEntity
        {
            UserName = signupDto.UserName,
        };

        var createResult = await userManager.CreateAsync(user, signupDto.Password);
        return createResult.Succeeded ? await GenerateAuthDtoForUserAsync(user) : FailureDto.BadRequest("Cannot signup.");
    }

    public async Task<Result<AuthDto, FailureDto>> LoginAsync(LoginDto loginDto)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.UserName);
        if (user is null) return FailureDto.Unauthorized("Invalid username or password.");

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        return isCorrectPassword ? await GenerateAuthDtoForUserAsync(user) : FailureDto.Unauthorized("Invalid username or password");
    }

    private async Task<AuthDto> GenerateAuthDtoForUserAsync(UserEntity user)
    {
        var refreshToken = new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.Add(_jwtConfiguration.RefreshTokenLifetime),
        };

        await dbContext.RefreshTokens.AddAsync(refreshToken);
        await dbContext.SaveChangesAsync();

        return CreateAuthDto(user, refreshToken.Token, tokenProvider.CreateToken(user));
    }

    private static AuthDto CreateAuthDto(UserEntity user, string refreshToken, string token) => new()
    {
        RefreshToken = refreshToken,
        Token = token,
        User = user.ToUserDto(),
    };

    public Task<Result<AuthDto, FailureDto>> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<Result<AuthDto, FailureDto>> MeAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}