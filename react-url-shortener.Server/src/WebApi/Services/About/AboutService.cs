using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.About;

/// <inheritdoc />
public class AboutService(string rootPath) : IAboutService
{
    private readonly string AboutAlgPath = Path.Combine(rootPath, "files", "about-alg.txt");

    /// <inheritdoc />
    public async Task<Result<string, FailureDto>> GetAboutAsync()
    {
        try
        {
            var about = await File.ReadAllTextAsync(AboutAlgPath);
            return about;
        }
        catch
        {
            return FailureDto.BadRequest(AboutServiceMessages.CANNOT_GET_ABOUT);
        }
    }

    /// <inheritdoc />
    public async Task<Result<string, FailureDto>> UpdateAboutAsync(string about)
    {
        try
        {
            await File.WriteAllTextAsync(AboutAlgPath, about);
            return about;
        }
        catch
        {
            return FailureDto.BadRequest(AboutServiceMessages.CANNOT_UPDATE_ABOUT);
        }
    }
}
