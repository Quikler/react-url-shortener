using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1;

public class BaseApiController : ControllerBase
{
    protected string RootApiUrl => $"{Request.Scheme}://{Request.Host}";
}