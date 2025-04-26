using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Contracts.V1.Responses;

namespace WebApi.Filters;

public class ValidateModelStateFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Values
                .SelectMany(x => x.Errors.Select(xx => xx.ErrorMessage))
                .ToList();

            context.Result = new BadRequestObjectResult(new FailureResponse(errors));
        }
    }
}
