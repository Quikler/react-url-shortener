using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Contracts;

namespace WebApi.Controllers.V1;

public class AboutController : BaseApiController
{
    [HttpGet(ApiRoutes.About.Get)]
    public async Task<IActionResult> GetAbout()
    {
        try
        {
            var about = await System.IO.File.ReadAllTextAsync("about-alg.txt");
            return Ok(about);
        }
        catch
        {
            return BadRequest("Cannot get about.");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut(ApiRoutes.About.Update)]
    public async Task<IActionResult> UpdateAbout([FromQuery] string about)
    {
        try
        {
            await System.IO.File.WriteAllTextAsync("about-alg.txt", about);
            return Ok(about);
        }
        catch
        {
            return BadRequest("Cannot update about.");
        }
    }
}