using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetify.Pulse
{
    internal class PulseMiddleware
    {
        private readonly RequestDelegate _next;

        public PulseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request.Path.ToString().EndsWith("/pulse"))
            {
                var executingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                using (var reader = new StreamReader(File.OpenRead($"{executingDirectory}/pulse-ui/index.html")))
                    await httpContext.Response.WriteAsync(reader.ReadToEnd());
            }
            await _next(httpContext);
        }
    }
}