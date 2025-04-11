using WebApi.Common;
using WebApi.DTOs;

namespace WebApi.Services.About;

/// <summary>
/// Provides functionality for retrieving and updating the content of the "about-alg.txt" file.
/// </summary>
/// <remarks>
/// The file is expected to be located in the "files" directory under the application's root path.
/// This service is intended to manage textual information related to the "About Algorithm" section.
/// </remarks>
public interface IAboutService
{
    /// <summary>
    /// Asynchronously retrieves the contents of the "about-alg.txt" file.
    /// </summary>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation, 
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing the file content as a string on success,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If the file cannot be read (e.g., due to it being missing, inaccessible, or corrupted),
    /// a <see cref="FailureDto.BadRequest(string)"/> with a bad request message is returned.
    /// </remarks>
    Task<Result<string, FailureDto>> GetAboutAsync();

    /// <summary>
    /// Asynchronously updates the contents of the "about-alg.txt" file with new text.
    /// </summary>
    /// <param name="about">The new content to write into the file.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation,
    /// with a result of <see cref="Result{TSuccess, TFailure}"/> containing the written content on success,
    /// or a <see cref="FailureDto"/> on failure.
    /// </returns>
    /// <remarks>
    /// If the file cannot be written (e.g., due to permission issues or disk errors),
    /// a <see cref="FailureDto.BadRequest(string)"/> with a bad request message is returned.
    /// </remarks>
    Task<Result<string, FailureDto>> UpdateAboutAsync(string about);
}
