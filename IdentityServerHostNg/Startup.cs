using System;
using IdentityServer.Extensions;
using IdentityServer.Host.Ng.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Host.Ng
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        private IWebHostEnvironment Environment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration conf)
        {
            Environment = env;
            Configuration = conf;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAntiForgeryConfiguration();
            services.AddIdentityServerConfiguration(Configuration, Environment);

            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration => configuration.RootPath = "wwwroot");
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                /*app.UseHsts();*/
            }

            // Registered before static files to always set header.
            app.UseNwebsec();
            app.UseHttpsRedirection();
            app.UseCorsConfiguration(Configuration);

            // app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            Environment.UseIdentityServerWebRoot();

            if (!Environment.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseIdentityServerConfiguration();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

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
