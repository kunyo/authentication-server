using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;

namespace AuthenticationService
{
    public static class AuthenticationServiceConfiguration
    {
        internal static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new[] {
                new IdentityResource("test-identity-resource", new[]{"claim1", "claim2", "claim3" })
            };
        }

        internal static IEnumerable<ApiResource> GetApis()
        {
            var apiResource = new ApiResource("test-api", new[] { "claim1", "claim2", "claim3" });
            apiResource.ApiSecrets = new[] { new Secret("test-api-secret".Sha512()) };
            return new[] { apiResource };
        }

        internal static IEnumerable<Client> GetClients()
        {
            return new[] {
                new Client{
                    ClientId = "client-1",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { "test-api" },
                    ClientSecrets = {
                        new Secret("client-1-secret".Sha512())
                    }
                }
            };
        }
    }
}
