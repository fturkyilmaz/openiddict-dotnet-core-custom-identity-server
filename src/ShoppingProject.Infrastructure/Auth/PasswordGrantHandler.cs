
using OpenIddict.Abstractions;
using OpenIddict.Server;
using ShoppingProject.UseCases.Users.Login;
using System.Security.Claims;
using OpenIddict.Server.AspNetCore;


namespace ShoppingProject.Infrastructure.Auth;

public sealed class PasswordGrantHandler
    : IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
{
    private readonly IMediator _mediator;

    public PasswordGrantHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async ValueTask HandleAsync(
        OpenIddictServerEvents.HandleTokenRequestContext context)
    {
        // if (!context.Request.IsPasswordGrantType())
        //     return;

        var result = await _mediator.Send(new LoginUserCommand(
            context.Request.Username!,
            context.Request.Password!
        ));

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, result.UserId.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Name, result.UserName);
        identity.AddClaim(OpenIddictConstants.Claims.Email, result.Email);

        foreach (var role in result.Roles)
            identity.AddClaim(new Claim(OpenIddictConstants.Claims.Role, role));

        var principal = new ClaimsPrincipal(identity);

        principal.SetScopes(context.Request.GetScopes());
        principal.SetResources("api");

        context.Principal = principal;
    }
}
