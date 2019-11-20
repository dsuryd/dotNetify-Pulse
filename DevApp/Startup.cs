using DotNetify.Pulse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DevApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDotNetifyPulse();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
#endif
            app.UseDotNetifyPulse(config =>
            {
                //config.UIPath = $"{Directory.GetCurrentDirectory()}/wwwroot/pulse-ui";
            });
            app.UseMvc();
        }
    }
}