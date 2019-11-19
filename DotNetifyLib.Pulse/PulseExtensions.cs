using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace DotNetify.Pulse
{
    public static class PulseExtensions
    {
        public static ILoggingBuilder AddDotNetifyPulse(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, PulseLoggerProvider>());
            return builder;
        }

        public static IApplicationBuilder UseDotNetifyPulse(this IApplicationBuilder app)
        {
            app.UseMiddleware<PulseMiddleware>();
            return app;
        }
    }
}