using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace OjtProgramApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        private ILogger<T>? logger;
        protected ILogger<T>? Logger =>
            logger ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    }
}
