using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer.Filters;
using IdentityServer.Models;
using IdentityServer.Models.Account;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CA1054 // Uri parameters should not be strings

namespace IdentityServer.Controllers
{
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly TestUserStore testUserStore;
        private readonly IIdentityServerInteractionService interaction;
        private readonly IEventService events;

        public AccountController(
            TestUserStore testUserStore,
            IIdentityServerInteractionService interaction,
            IEventService events)
        {
            this.testUserStore = testUserStore;
            this.interaction = interaction;
            this.events = events;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            return View(await BuildLoginViewModelAsync(returnUrl));
        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl)
        {
            // ViewData["ReturnUrl"] = returnUrl;
            // return View();
            return Ok("Access denied: " + returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var returnUrl = model.ReturnUrl;
            if (ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;

                // validate username/password against in-memory store
                if (testUserStore.ValidateCredentials(model.Username, model.Password))
                {
                    var user = testUserStore.FindByUsername(model.Username);
                    await events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username));

                    // issue authentication cookie with subject ID and username
                    // await HttpContext.SignInAsync(user.SubjectId, user.Username);
                    await HttpContext.SignInAsync(new ClaimsPrincipal());

                    // make sure the returnUrl is still valid, and if so redirect back to authorize endpoint or a local page
                    // the IsLocalUrl check is only necessary if you want to support additional local pages, otherwise IsValidReturnUrl is more strict
                    if (interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                        return Redirect(model.ReturnUrl);

                    return Redirect("~/");
                }

                await events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
            }

            // Something went wrong, show form with error.
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }

        private async Task<LoginInputModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await interaction.GetAuthorizationContextAsync(returnUrl);
            return new LoginInputModel
            {
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
            };
        }

        private async Task<LoginInputModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            return vm;
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            return await Logout(new LogoutInputModel { LogoutId = logoutId });
        }

        // ReSharper disable once UnusedParameter.Global
        #pragma warning disable CA1801 // Review unused parameters
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            /*var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);*/
            if (User?.Identity.IsAuthenticated == true)
            {
                // Delete local authentication cookie.
                await HttpContext.SignOutAsync();

                // uncomment this when services.AddAuthentication() options are uncommented
                // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                // await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                await events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

                // Set this so UI rendering sees an anonymous user.
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return Redirect("~/");
            /*return View("LoggedOut", vm); // "LoggedOut", vm);*/
        }

        /*private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // Get context information (client name, post logout redirect URI and iframe for federated signout).
            var logout = await interaction.GetLogoutContextAsync(logoutId);

            return new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };
        }*/
    }
}
