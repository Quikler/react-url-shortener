using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.About;

public class AboutService(string rootPath) : IAboutService
{
    private readonly string AboutAlgPath = Path.Combine(rootPath, "files", "about-alg.txt");

    public async Task<Result<string, FailureDto>> GetAboutAsync()
    {
        try
        {
            var about = await File.ReadAllTextAsync(AboutAlgPath);
            return about;
        }
        catch
        {
            return FailureDto.BadRequest("Cannot update about.");
        }
    }

    public async Task<Result<string, FailureDto>> UpdateAboutAsync(string about)
    {
        try
        {
            await File.WriteAllTextAsync(AboutAlgPath, about);
            return about;
        }
        catch
        {
            return FailureDto.BadRequest("Cannot update about.");
        }
    }
}