using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Extensions
{
    /// <summary>
    /// Adds MbsApi services to the dependency injection container.
    /// </summary>
    public static class IdentityServerConfigurationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MbsApi services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">A collection of services.</param>
        /// <param name="configuration">A configuration interface.</param>
        /// <param name="environment">An environment interface.</param>
        /// <returns>An updated service collection.</returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static IServiceCollection AddIdentityServerConfiguration(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment environment)
        {
            Config.HostUrl = configuration.GetSection("urls").Get<string>();

            Tuple<X509Certificate2, X509Certificate2> certs = GetTokenCertificates(configuration, environment);

            services
                .AddIdentityServer() /*options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                })*/
                .AddSigningCredential(certs.Item1)
                .AddValidationKey(certs.Item2)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers())
                .AddProfileService<ProfileService>();

            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Config.AuthenticatedPolicy, policy =>
                    {
                        policy.RequireAuthenticatedUser();
                    });
                    options.AddPolicy(Config.AdministrationApiPolicy, policy =>
                    {
                        policy.RequireClaim(Config.ApiLevel, Config.AdministrationApi);
                    });
                    options.AddPolicy(Config.BasicApiPolicy, policy =>
                    {
                        policy.RequireClaim(Config.ApiLevel, Config.BasicApi);
                    });
                    options.AddPolicy(Config.AdvancedApiPolicy, policy =>
                    {
                        policy.RequireClaim(Config.ApiLevel, Config.AdvancedApi);
                    });
                });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication() /*options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })*/
                /*.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = "ids_auth";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.Cookie.SameSite = SameSiteMode.None;
                })*/
                /*.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

                    options.Authority = Config.HostUrl;
                    options.RequireHttpsMetadata = false;

                    options.ClientId = Config.ClientId;
                    options.ResponseType = "token_id token";
                    options.SaveTokens = true;
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SecurityTokenValidator = new JwtSecurityTokenHandler { InboundClaimTypeMap = new Dictionary<string, string>() };
                    options.ClaimActions.MapJsonKey(Config.ApiLevel, Config.ApiLevel);
                })*/
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Config.HostUrl;
                    options.RequireHttpsMetadata = false;
                    options.ApiName = Config.ApiLevel;
                    options.SupportedTokens = SupportedTokens.Both;
                    options.SaveToken = true;
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10);
                 });

            return services;
        }

        private static Tuple<X509Certificate2, X509Certificate2> GetTokenCertificates(IConfiguration configuration, IHostingEnvironment environment)
        {
            IConfigurationSection configurationSection = configuration.GetSection("TokenCertificates");
            var useLocalCertStore = configurationSection.GetSection("UseLocalCertStore").Get<bool>();
            if (useLocalCertStore)
            {
                var signingThumbprint = configurationSection.GetSection("SigningThumbprint").Get<string>();
                var validationThumbprint = configurationSection.GetSection("ValidationThumbprint").Get<string>();
                return new Tuple<X509Certificate2, X509Certificate2>(
                    signingThumbprint.GetX509Certificate(),
                    validationThumbprint.GetX509Certificate());
            }

            var password = configurationSection.GetSection("PfxPassword").Get<string>();
            var signingPfx = configurationSection.GetSection("SigningPfx").Get<string>();
            var validationPfx = configurationSection.GetSection("ValidationPfx").Get<string>();

            return new Tuple<X509Certificate2, X509Certificate2>(
                new X509Certificate2(Path.Combine(environment.ContentRootPath, signingPfx), password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet),
                new X509Certificate2(Path.Combine(environment.ContentRootPath, validationPfx), password, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet));
        }
    }
}