using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IdentityServer.Filters
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ViewResult)
            {
                const string xContentTypeOptions = "X-Content-Type-Options";
                const string xFrameOptions = "X-Frame-Options";
                const string xContentSecurityPolicy = "X-Content-Security-Policy";
                const string referrerPolicy = "Referrer-Policy";
                const string contentSecurityPolicy = "Content-Security-Policy";
                const string csp = "default-src 'self'; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';"; // img-src 'self;'

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey(xContentTypeOptions))
                    context.HttpContext.Response.Headers.Add(xContentTypeOptions, "nosniff");

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey(xFrameOptions))
                    context.HttpContext.Response.Headers.Add(xFrameOptions, "SAMEORIGIN");

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                if (!context.HttpContext.Response.Headers.ContainsKey(referrerPolicy))
                    context.HttpContext.Response.Headers.Add(referrerPolicy, "no-referrer");

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                if (!context.HttpContext.Response.Headers.ContainsKey(contentSecurityPolicy))
                    context.HttpContext.Response.Headers.Add(contentSecurityPolicy, csp);
                if (!context.HttpContext.Response.Headers.ContainsKey(xContentSecurityPolicy)) // And once again for IE.
                    context.HttpContext.Response.Headers.Add(xContentSecurityPolicy, csp);
            }
        }
    }
}
