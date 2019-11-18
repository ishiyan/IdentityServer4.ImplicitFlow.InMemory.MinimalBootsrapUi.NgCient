using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace IdentityServer.Extensions
{
    /// <summary>
    /// Registers MbsApi CORS middleware in the Http pipeline.
    /// </summary>
    public static class IdentityServerConfigurationApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the middleware to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// </summary>
        /// <param name="app">An application builder instance.</param>
        /// <returns>An updated application builder instance.</returns>
        public static IApplicationBuilder UseIdentityServerConfiguration(this IApplicationBuilder app)
        {
            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        /// <summary>
        /// Extends web root provider to include static files in IdentityServer wwwroot folder.
        /// </summary>
        /// <param name="env">A web host environment instance.</param>
        public static void UseIdentityServerWebRoot(this IWebHostEnvironment env)
        {
            var assembly = typeof(Models.Account.LoginInputModel).Assembly;
            var fileProviders = new List<IFileProvider>
            {
                env.WebRootFileProvider,
                new ManifestEmbeddedFileProvider(assembly, "wwwroot")
            };

            env.WebRootFileProvider = new CompositeFileProvider(fileProviders);
        }
    }
}
