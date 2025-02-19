using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.About;

public interface IAboutService
{
    Task<Result<string, FailureDto>> GetAboutAsync();
    Task<Result<string, FailureDto>> UpdateAboutAsync(string about);
}