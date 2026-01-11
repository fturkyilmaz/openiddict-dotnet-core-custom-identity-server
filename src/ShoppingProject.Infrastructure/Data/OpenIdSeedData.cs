using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShoppingProject.Infrastructure.Data
{
    public static class OpenIdSeedData
    {
        public static async Task InitializeAsync(IServiceProvider provider)
        {
            using var scope = provider.CreateScope();

            await PopulateScopes(scope);

            await PopulateClientApp(scope);

            await PopulateServiceApp(scope);
        }

        private static async ValueTask PopulateScopes(IServiceScope scope)
        {
            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

            var scopes = new[]
            {
                new OpenIddictScopeDescriptor
                {
                    Name = "clientapi",
                    DisplayName = "Client Api Access",
                    Resources = { "api" }
                },
                new OpenIddictScopeDescriptor
                {
                    Name = "postman",
                    DisplayName = "Postman Access",
                    Resources = { "api" }
                }
            };

            foreach (var scopeDescriptor in scopes)
            {
                var scopeInstance = await scopeManager.FindByNameAsync(scopeDescriptor.Name!, default);

                if (scopeInstance == null)
                {
                    await scopeManager.CreateAsync(scopeDescriptor);
                }
                else
                {
                    await scopeManager.UpdateAsync(scopeInstance, scopeDescriptor);
                }
            }
        }

        private static async ValueTask PopulateClientApp(IServiceScope scopeService)
        {
            var appManager = scopeService.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var appDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "open-id-api",
                ClientSecret = "open-id-api-secret",
                DisplayName = "OpenIDApi",
                RedirectUris = { new Uri("https://localhost:7085/swagger/oauth2-redirect.html") },
                ClientType = ClientTypes.Confidential,
                ConsentType = ConsentTypes.Explicit,
                Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Introspection,
                        Permissions.Endpoints.Revocation,

                        Permissions.ResponseTypes.Code,

                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Email,
                        Permissions.Prefixes.Scope + "clientapi",

                    }
            };

            var client = await appManager.FindByClientIdAsync(appDescriptor.ClientId);

            if (client == null)
            {
                await appManager.CreateAsync(appDescriptor);
            }
            else
            {
                await appManager.UpdateAsync(client, appDescriptor);
            }
        }

        private static async ValueTask PopulateServiceApp(IServiceScope scopeService )
        {
            var appManager = scopeService.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            var appDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = "open-id-postman",
                ClientSecret = "open-id-postman-secret",
                DisplayName = "OpenIDPostman",
                RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                ClientType = ClientTypes.Confidential,
                ConsentType = ConsentTypes.Explicit,
                Permissions =
                    {
                        Permissions.Endpoints.Token,
                        Permissions.Endpoints.Authorization,
                        Permissions.Endpoints.Introspection,
                        Permissions.Endpoints.Revocation,

                        Permissions.ResponseTypes.Code,

                        Permissions.GrantTypes.ClientCredentials,
                        Permissions.GrantTypes.AuthorizationCode,
                        Permissions.GrantTypes.RefreshToken,

                        Permissions.Scopes.Profile,
                        Permissions.Scopes.Email,
                        Permissions.Prefixes.Scope + "postman"
                    }
            };

            var client = await appManager.FindByClientIdAsync(appDescriptor.ClientId);

            if (client == null)
            {
                await appManager.CreateAsync(appDescriptor);
            }
            else
            {
                await appManager.UpdateAsync(client, appDescriptor);
            }
        }
    }
}