/*
Copyright 2019 Dicky Suryadi
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

using System;
using System.Collections.Generic;
using DotNetify.Pulse.Log;
using DotNetify.Pulse.SystemUsage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotNetify.Pulse
{
   public static class PulseExtensions
   {
      public static IServiceCollection AddDotNetifyPulse(this IServiceCollection services, IConfiguration configuration = null)
      {
         var pulseConfig = configuration?.GetSection(PulseConfiguration.SECTION).Get<PulseConfiguration>() ?? new PulseConfiguration();
         services.TryAdd(ServiceDescriptor.Singleton(_ => pulseConfig));

         return services
            .AddPulseLogger()
            .AddPulseMemoryUsage()
            .AddPulseCpuUsage();
      }

      public static IServiceCollection ClearPulseDataProvider(this IServiceCollection services)
      {
         return services.RemoveAll<IPulseDataProvider>();
      }

      public static IDotNetifyConfiguration UseDotNetifyPulse(this IDotNetifyConfiguration dotNetifyConfig, IApplicationBuilder app, Action<PulseConfiguration> options = null)
      {
         var pulseConfig = app.ApplicationServices.GetRequiredService<PulseConfiguration>();
         options?.Invoke(pulseConfig);

         dotNetifyConfig.RegisterAssembly(typeof(PulseVM).Assembly);
         app.UseMiddleware<PulseMiddleware>();
         return dotNetifyConfig;
      }

      public static void AddTo<T>(this T item, List<T> items)
      {
         items.Add(item);
      }
   }
}