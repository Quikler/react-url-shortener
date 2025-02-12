using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Identity;

namespace WebApi.Services.Identity;

public interface IIdentityService
{
    Task<Result<AuthDto, FailureDto>> LoginAsync(LoginDto loginDto);
    Task<Result<AuthDto, FailureDto>> SignupAsync(SignupDto signupDto);
    Task<Result<AuthDto, FailureDto>> RefreshTokenAsync(string refreshToken);
    Task<Result<AuthDto, FailureDto>> MeAsync(string refreshToken);
}