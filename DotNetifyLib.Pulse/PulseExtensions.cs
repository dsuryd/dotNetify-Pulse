using DotNetify.Pulse.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetify.Pulse
{
   public static class PulseExtensions
   {
      private static readonly string CONFIG_SECTION = "DotNetifyPulse";

      public static ILoggingBuilder AddDotNetifyPulse(this ILoggingBuilder builder, IConfiguration config = null)
      {
         builder.AddConfiguration();

         var pulseConfig = config?.GetSection(CONFIG_SECTION).Get<PulseConfiguration>() ?? new PulseConfiguration();
         builder.Services.TryAdd(ServiceDescriptor.Singleton(_ => pulseConfig));

         builder.Services.TryAdd(ServiceDescriptor.Transient(_ => new LoggerExternalScopeProvider()));

         builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, PulseLoggerProvider>());
         builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IDataSource, PulseLogger>());
         builder.Services.TryAdd(ServiceDescriptor.Singleton(provider => (IPulseLogger) provider.GetServices<IDataSource>().First(x => x is PulseLogger)));

         builder.Services.AddHostedService<SystemUsageCollector>();

         return builder;
      }

      public static IApplicationBuilder UseDotNetifyPulse(this IApplicationBuilder app, Action<PulseConfiguration> options = null)
      {
         var pulseConfig = app.ApplicationServices.GetRequiredService<PulseConfiguration>();
         options?.Invoke(pulseConfig);

         VMController.RegisterAssembly(typeof(PulseVM).Assembly);
         app.UseMiddleware<PulseMiddleware>();
         return app;
      }

      public static void AddTo<T>(this T item, List<T> items)
      {
         items.Add(item);
      }
   }
}