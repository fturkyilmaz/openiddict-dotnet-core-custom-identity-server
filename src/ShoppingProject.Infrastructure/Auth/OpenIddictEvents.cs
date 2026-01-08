using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using ShoppingProject.Infrastructure.Auth;

namespace ShoppingProject.Infrastructure.Auth;

public static class OpenIddictEvents
{
    public static void Configure(OpenIddictServerBuilder options)
    {
        options.AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(builder =>
        {
            builder.UseInlineHandler(async context =>
            {
                // if (!context.Request.IsPasswordGrantType())
                //     return;

                // var username = context.Request.Username;
                // var password = context.Request.Password;

                // var userService = context.Transaction.Services.GetRequiredService<IAuthService>();


                // var user = await userService.ValidateAsync(username!, password!);
                // if (user is null)
                // {
                //     context.Reject(
                //         error: OpenIddictConstants.Errors.InvalidGrant,
                //         description: "Invalid credentials.");
                //     return;
                // }

                // var identity = new ClaimsIdentity(
                //     TokenValidationParameters.DefaultAuthenticationType,
                //     nameType: ClaimTypes.Name,
                //     roleType: ClaimTypes.Role);

                // identity.AddClaim(new Claim(OpenIddictConstants.Claims.Subject, user.Id.ToString()));
                // identity.AddClaim(new Claim(OpenIddictConstants.Claims.Email, user.Email));
                // identity.AddClaim(new Claim(OpenIddictConstants.Claims.Name, user.UserName));

                // var principal = new ClaimsPrincipal(identity);

                // principal.SetScopes(new[]
                // {
                //     OpenIddictConstants.Scopes.OpenId,
                //     OpenIddictConstants.Scopes.Email,
                //     OpenIddictConstants.Scopes.Profile,
                //     "api"
                // });

                // context.SignIn(principal);
            });
        });
    }
}
