using DotNetify;
using DotNetify.Pulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DevApp
{
   public class Startup
   {
      public IConfiguration Configuration { get; }

      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public void ConfigureServices(IServiceCollection services)
      {
         services.AddSignalR();
         services.AddDotNetify().AddDotNetifyPulse(Configuration);

         services.AddHostedService<TestHostedService>();
         services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      }

      public void Configure(IApplicationBuilder app)
      {
         app.UseWebSockets();
         app.UseSignalR(config => config.MapDotNetifyHub());
         app.UseDotNetify(config =>
         {
            config.UseDotNetifyPulse(app, pulse =>
            {
               pulse.UIPath = $"{Directory.GetCurrentDirectory()}\\custom-pulse-ui";
            });
         });

         app.UseStaticFiles();
         app.UseMvc();
      }
   }
}