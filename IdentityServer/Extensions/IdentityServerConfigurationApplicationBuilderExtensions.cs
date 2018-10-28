using Microsoft.AspNetCore.Builder;

namespace IdentityServer.Extensions
{
    /// <summary>
    /// Registers MbsApi CORS middleware in the Http pipeline.
    /// </summary>
    public static class IdentityServerConfigurationApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the MbsApi CORS middleware to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        /// <param name="app">An application builder instance.</param>
        /// <returns>An updated application builder instance.</returns>
        public static IApplicationBuilder UseIdentityServerConfiguration(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
            return app;
        }
    }
}
