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
            var uiPath = (_config.UIPath ?? $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\pulse-ui").TrimEnd(new char[] { '/', '\\' });
            var requestPath = httpContext.Request.Path.ToString();

            if (requestPath.EndsWith("/pulse"))
            {
                string body = File.ReadAllText($"{uiPath}/body.html");
                string index = File.ReadAllText($"{uiPath}/index.html").Replace("@body", body);
                await httpContext.Response.WriteAsync(index);
            }
            else
                await _next(httpContext);
        }
    }
}