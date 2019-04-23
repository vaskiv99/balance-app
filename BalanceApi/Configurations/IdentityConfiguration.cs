using System.Collections.Generic;
using IdentityServer4.Models;

namespace Balance.Api.Configurations
{
    public static class IdentityConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource {
                    Name = "BalanceApi",
                    DisplayName ="Balance API",
                    Description = "Balance API Access",
                    ApiSecrets = new List<Secret> {new Secret("BalanceApiSecret".Sha256())},
                    Scopes = new List<Scope> {
                        new Scope("BalanceApiSecret.read"),
                        new Scope("BalanceApi.write"),
                        new Scope("offline_access")
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client
                {
                    ClientId = "web_client",
                    ClientName = "Web Client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("BalanceApiSecret".Sha256())
                    },
                    AllowedScopes = new List<string> { "BalanceApiSecret.read", "offline_access" },
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = 100000
                }
            };
        }
    }
}