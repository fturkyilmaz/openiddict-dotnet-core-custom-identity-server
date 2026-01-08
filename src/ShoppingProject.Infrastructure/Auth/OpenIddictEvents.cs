using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ShoppingProject.Infrastructure.Auth;

public static class OpenIddictEvents
{
    public static void Configure(OpenIddictServerBuilder options)
    {
        options.AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(builder =>
        {
            builder.UseInlineHandler(async context =>
            {
                if (!context.Request.IsPasswordGrantType())
                    return;

                var username = context.Request.Username;
                var password = context.Request.Password;

                var userService = context.Transaction.GetHttpContext()!
                    .RequestServices.GetRequiredService<IUserAuthService>();

                var user = await userService.ValidateAsync(username!, password!);
                if (user is null)
                {
                    context.Reject(
                        error: OpenIddictConstants.Errors.InvalidGrant,
                        description: "Invalid credentials.");
                    return;
                }

                var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType);
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()));
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Email, user.Email));
                identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, user.DisplayName));

                var principal = new ClaimsPrincipal(identity);

                principal.SetScopes(new[]
                {
                    OpenIddictConstants.Scopes.Email,
                    OpenIddictConstants.Scopes.Profile,
                    "api"
                });

                await context.SignInAsync(principal);
            });
        });
    }
}
