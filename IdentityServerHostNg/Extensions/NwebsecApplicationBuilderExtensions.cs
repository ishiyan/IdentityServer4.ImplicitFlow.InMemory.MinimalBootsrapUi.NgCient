using Microsoft.AspNetCore.Builder;

namespace IdentityServer.Host.Ng.Extensions
{
    /// <summary>
    /// Adds the NWebSec middlewares to the <see cref="IApplicationBuilder"/> request execution pipeline.
    /// </summary>
    public static class NwebsecApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds the NWebSec middlewares to the <see cref="IApplicationBuilder"/> request execution pipeline.
        /// Should be registered before static files to always set header.
        /// See https://docs.nwebsec.com/en/latest/index.html
        /// </summary>
        /// <param name="app">The application builder interface.</param>
        /// <returns>An updated application builder instance.</returns>
        public static IApplicationBuilder UseNwebsec(this IApplicationBuilder app)
        {
            app.UseHsts(hsts => hsts.MaxAge(365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.NoReferrer());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseXfo(options => options.Deny());

            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .FontSources(s => s.Self().CustomSources("fonts.googleapis.com", "fonts.gstatic.com"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self()));
            /* .StyleSources(s => s.Self().CustomSources("fonts.googleapis.com")) */
            /* .ImageSources(s => s.Self()) */
            /* .ScriptSources(s => s.Self()) */

            return app;
        }
    }
}