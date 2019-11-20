using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetify.Pulse
{
    internal class PulseMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PulseConfiguration _config;

        public PulseMiddleware(RequestDelegate next, PulseConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var uiPath = (_config.UIPath ?? $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}/pulse-ui").TrimEnd('/');
            var requestPath = httpContext.Request.Path.ToString();

            if (requestPath.EndsWith("/pulse"))
            {
                using (var reader = new StreamReader(File.OpenRead($"{uiPath}/index.html")))
                    await httpContext.Response.WriteAsync(reader.ReadToEnd());
            }
            else if (requestPath.EndsWith("/pulse-ui/main.js"))
            {
                using (var reader = new StreamReader(File.OpenRead($"{uiPath}/main.js")))
                    await httpContext.Response.WriteAsync(reader.ReadToEnd());
            }
            else
                await _next(httpContext);
        }
    }
}