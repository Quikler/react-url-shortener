using WebApi.Common;
using WebApi.DTOs;
using WebApi.DTOs.Identity;

namespace WebApi.Services.Identity;

/// <summary>
/// Provides functionality for managing user authentication, identification and registration
/// </summary>
public interface IIdentityService
{
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
    Task<Result<AuthDto, FailureDto>> SignupAsync(SignupDto signupDto);
    
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
    Task<Result<AuthDto, FailureDto>> LoginAsync(LoginDto loginDto);

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
    Task<Result<AuthDto, FailureDto>> RefreshTokenAsync(string refreshToken);

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
    Task<Result<AuthDto, FailureDto>> MeAsync(string refreshToken);
}
