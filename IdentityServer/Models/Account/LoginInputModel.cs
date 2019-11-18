using System.ComponentModel.DataAnnotations;

#pragma warning disable CA1056 // Uri properties should not be strings

namespace IdentityServer.Models.Account
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}