using Microsoft.AspNetCore.Mvc;
using WebApi.Common;
using WebApi.DTOs;
using WebApi.Mapping;

namespace WebApi.Utils.Extensions;

public static class FailureDtoExtensions
{
    public static IActionResult ToActionResult(this FailureDto failure)
    {
        var response = failure.ToResponse();

        var failureMapping = new Dictionary<FailureCode, IActionResult>
        {
            { FailureCode.BadRequest, new BadRequestObjectResult(response) },
            { FailureCode.Unauthorized, new UnauthorizedObjectResult(response) },
            { FailureCode.Forbidden, new ForbidResult() },
            { FailureCode.NotFound, new NotFoundObjectResult(response) },
            { FailureCode.Conflict, new ConflictObjectResult(response) },
        };

        return failureMapping.TryGetValue(failure.FailureCode, out var result)
            ? result
            : new ObjectResult(response) { StatusCode = StatusCodes.Status500InternalServerError };
    }
}
