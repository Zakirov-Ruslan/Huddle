using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Huddle.Identity.Configs
{
    public static class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            string spaUrl = Environment.GetEnvironmentVariable("SPA_URL") 
                ?? throw new ArgumentNullException("No SPA_URL environment variable defined");

            return new List<Client>
            {
                new Client
                {
                    ClientId = "interactive.confidential",
                    ClientName = "React SPA",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { spaUrl + "/callback" },
                    PostLogoutRedirectUris = { spaUrl },
                    AllowedCorsOrigins = { spaUrl },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1",
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },

                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 60,
                    AbsoluteRefreshTokenLifetime = 24 * 60 * 60,
                    SlidingRefreshTokenLifetime = 12 * 60 * 60,

                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding
                }
            };
        }

        public static IEnumerable<IdentityResource> GetResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope("api1", "Sample API")
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("huddle.channel.api", "Huddle Channel API")
            };
        }
    }
}
