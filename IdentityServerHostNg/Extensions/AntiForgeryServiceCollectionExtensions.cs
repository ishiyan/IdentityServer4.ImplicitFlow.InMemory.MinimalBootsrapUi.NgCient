using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Host.Ng.Extensions
{
    public static class AntiForgeryServiceCollectionExtensions
    {
        public static IServiceCollection AddAntiForgeryConfiguration(this IServiceCollection services)
        {
            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "idsaf";
                options.Cookie.HttpOnly = true;
                options.HeaderName = "X-XSRF-TOKEN";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            return services;
        }
    }
}