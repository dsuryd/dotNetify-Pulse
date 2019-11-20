using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace DevApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get([FromQuery] string level, [FromServices] ILogger<TestController> logger)
        {
            switch (level)
            {
                case "critical": logger.LogCritical("Critical level is logged"); break;
                case "error": logger.LogError("Error level is logged"); break;
                case "warning": logger.LogWarning("Warning level is logged"); break;
                case "trace": logger.LogTrace("Trace level is logged"); break;
                case "debug": logger.LogDebug("Debug level is logged"); break;
                default: logger.LogInformation("Info level is logged"); break;
            }
            return "200 OK";
        }
    }
}