using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetify.Pulse
{
    internal class PulseMiddleware
    {
        private static readonly string DEFAULT_UI_PATH = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\pulse-ui";

        private readonly RequestDelegate _next;
        private readonly PulseConfiguration _config;

        public PulseMiddleware(RequestDelegate next, PulseConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var uiPath = (_config.UIPath ?? DEFAULT_UI_PATH).TrimEnd(new char[] { '/', '\\' });
            var requestPath = httpContext.Request.Path.ToString();

            if (requestPath.EndsWith("/pulse"))
            {
                string body = File.Exists($"{uiPath}\\body.html") ? File.ReadAllText($"{uiPath}\\body.html") : File.ReadAllText($"{DEFAULT_UI_PATH}\\body_default.html");
                string script = File.Exists($"{uiPath}\\script.html") ? File.ReadAllText($"{uiPath}\\script.html") : "";
                string index = File.Exists($"{uiPath}\\index.html") ? File.ReadAllText($"{uiPath}\\index.html") : File.ReadAllText($"{DEFAULT_UI_PATH}\\index.html");

                await httpContext.Response.WriteAsync(index
                    .Replace("<!--body-->", body)
                    .Replace("<!--script-->", script)
                );
            }
            else
                await _next(httpContext);
        }
    }
}