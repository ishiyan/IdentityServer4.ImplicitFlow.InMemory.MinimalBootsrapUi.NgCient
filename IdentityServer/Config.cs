using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

// ReSharper disable once ClassNeverInstantiated.Global
namespace IdentityServer
{
    public static class Config
    {
        private const string ClientId = "ngclient";

        public const string ApiLevel = "api_level";
        public const string AdministrationApi = "administration_api";
        public const string BasicApi = "basic_api";
        public const string AdvancedApi = "advanced_api";

        public const string AuthenticatedPolicy = "Authenticated";
        public const string AdministrationApiPolicy = "AdministrationApi";
        public const string AdvancedApiPolicy = "AdvancedApi";
        public const string BasicApiPolicy = "BasicApi";

        internal static string HostUrl;

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Profile();
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource(ApiLevel)
            {
                UserClaims = { AdministrationApi, BasicApi, AdvancedApi }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            yield return new Client
            {
                AllowedGrantTypes = GrantTypes.Implicit,
                ClientId = ClientId,
                AccessTokenType = AccessTokenType.Jwt,
                AccessTokenLifetime = 600, // 10 minutes, default 60 minutes
                AlwaysSendClientClaims = true,
                AlwaysIncludeUserClaimsInIdToken = true,
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                RedirectUris = new List<string>
                {
                    HostUrl,
                    HostUrl + "silent-refresh.html"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    HostUrl
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "offline_access",
                    ApiLevel
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "a",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Admin"),
                        new Claim(ApiLevel, AdministrationApi)
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "guest",
                    Password = "g",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Guest"),
                        new Claim(ApiLevel, BasicApi)
                    }
                },
                new TestUser
                {
                    SubjectId = "3",
                    Username = "user",
                    Password = "u",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "User"),
                        new Claim(ApiLevel, AdvancedApi),
                        new Claim(ApiLevel, BasicApi)
                    }
                }
            };
        }
    }
}
