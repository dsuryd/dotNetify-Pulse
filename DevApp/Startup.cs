using DotNetify;
using DotNetify.Pulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DevApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddDotNetify();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
#endif
            app.UseWebSockets();
            app.UseSignalR(config => config.MapDotNetifyHub());

            app.UseDotNetifyPulse(config =>
            {
                config.UIPath = $"{Directory.GetCurrentDirectory()}\\custom-pulse-ui";
            });
            app.UseDotNetify();

            app.UseMvc();
        }
    }
}