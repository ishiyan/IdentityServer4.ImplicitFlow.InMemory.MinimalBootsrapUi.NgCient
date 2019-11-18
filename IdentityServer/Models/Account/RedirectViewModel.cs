#pragma warning disable CA1056 // Uri properties should not be strings

namespace IdentityServer.Models.Account
{
    public class RedirectViewModel
    {
        public string RedirectUrl { get; set; }
    }
}