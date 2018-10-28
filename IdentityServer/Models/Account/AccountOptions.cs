#pragma warning disable CA2211 // Non-constant fields should not be visible

namespace IdentityServer.Models
{
    public static class AccountOptions
    {
        public static bool AutomaticRedirectAfterSignOut = true;

        public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    }
}
