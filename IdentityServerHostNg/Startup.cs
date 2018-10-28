using System;
using IdentityServer.Extensions;
using IdentityServer.Host.Ng.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
#pragma warning disable CA1822 // Mark members as static

namespace IdentityServer.Host.Ng
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env, IConfiguration conf)
        {
            Environment = env;
            Configuration = conf;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAntiForgeryConfiguration();
            services.AddIdentityServerConfiguration(Configuration, Environment);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "wwwroot");
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
                app.UseDeveloperExceptionPage();
            /*else
                app.UseExceptionHandler("/Error");*/

            // Registered before static files to always set header.
            app.UseNwebsec();
            app.UseCorsConfiguration(Configuration);
            app.UseIdentityServerConfiguration();
            /*app.UseHttpsRedirection();*/

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvcWithDefaultRoute();
            app.UseSpa(spa =>
            {
                // See https://go.microsoft.com/fwlink/?linkid=864501
                spa.Options.SourcePath = "ClientApp";
                spa.Options.StartupTimeout = new TimeSpan(0, 0, 2, 0);
                if (Environment.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");

                    // When you want to launch the application: cd ClientApp; ng serve; cd ..; dotnet run
                    // spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
        }
    }
}
