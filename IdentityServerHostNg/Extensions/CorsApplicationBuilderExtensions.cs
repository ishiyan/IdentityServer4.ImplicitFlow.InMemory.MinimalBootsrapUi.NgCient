using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IdentityServer.Host.Ng.Extensions
{
    public static class CorsApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCorsConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            IConfigurationSection configurationSection = configuration.GetSection("Cors");
            var origins = configurationSection.GetSection("Origins").Get<string[]>();
            var allowAll = configurationSection.GetSection("AllowAll").Get<bool>();

            app.UseCors(builder =>
            {
                if (allowAll)
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); // .AllowCredentials();
                }
                else if (origins != null && origins.Any())
                {
                    builder.WithOrigins(configurationSection.GetSection("Origins").Get<string[]>())
                        .AllowAnyHeader();
                }
            });

            return app;
        }
    }
}