using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bliscipline.OAuth.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[] {
                new ApiResource("bliscipline", "Bliscipline")
                {
                    UserClaims = new [] { "email" }
                }
            };
        }


        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[] {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[] {
                new Client
                {
                    ClientId = "bliscipline",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "bliscipline" }
                },

                new Client
                {
                    ClientId = "bliscipline_implicit",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = new [] {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "bliscipline"
                    },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:28849/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:28849/signout-callback-oidc" },
                },

                new Client
                {
                    ClientId = "bliscipline_code",
                    ClientSecrets = new [] { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowedScopes = new [] {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "bliscipline"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new [] { "http://localhost:28849/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:28849/signout-callback-oidc" },
                }
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[] {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "mail@filipekberg.se",
                    Password = "password",
                    Claims = new [] { new Claim("email", "mail@filipekberg.se") }
                }
            };
        }
    }
}
