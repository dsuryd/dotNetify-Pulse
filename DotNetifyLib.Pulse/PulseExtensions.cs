using DotNetify.Pulse.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace DotNetify.Pulse
{
    public static class PulseExtensions
    {
        public static ILoggingBuilder AddDotNetifyPulse(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, PulseLoggerProvider>());
            builder.Services.TryAdd(ServiceDescriptor.Singleton<ILogEmitter, LogEmitter>());
            return builder;
        }

        public static IServiceCollection AddDotNetifyPulse(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddDotNetify();
            return services;
        }

        public static IApplicationBuilder UseDotNetifyPulse(this IApplicationBuilder app, Action<PulseConfiguration> options = null)
        {
            var pulseConfig = new PulseConfiguration();
            options?.Invoke(pulseConfig);

            app.UseWebSockets();
            app.UseSignalR(routes => routes.MapDotNetifyHub());
            app.UseDotNetify(dotNetifyConfig =>
            {
                dotNetifyConfig.RegisterAssembly(typeof(PulseVM).Assembly);
            });

            app.UseMiddleware<PulseMiddleware>(pulseConfig);
            return app;
        }
    }
}