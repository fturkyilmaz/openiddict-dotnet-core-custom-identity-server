using OpenIddict.Abstractions;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using ShoppingProject.UseCases.Users.Commands.Login;
using System.Security.Claims;

namespace ShoppingProject.Infrastructure.Auth;

public sealed class PasswordGrantHandler
    : IOpenIddictServerHandler<OpenIddictServerEvents.HandleTokenRequestContext>
{
    private readonly IMediator _mediator;
    private readonly ILogger<PasswordGrantHandler> _logger;

    public PasswordGrantHandler(IMediator mediator, ILogger<PasswordGrantHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async ValueTask HandleAsync(OpenIddictServerEvents.HandleTokenRequestContext context)
    {
        // Sadece password grant için çalıştır
        if (!context.Request.IsPasswordGrantType())
            return;

        // Basit doğrulama
        if (string.IsNullOrWhiteSpace(context.Request.Username) ||
            string.IsNullOrWhiteSpace(context.Request.Password))
        {
            context.Reject(
                error: OpenIddictConstants.Errors.InvalidRequest,
                description: "Username or password is missing.");
            return;
        }

        // Kullanıcı doğrulama (LoginCommand içinde şifre kontrolü yapılmalı)
        var result = await _mediator.Send(new LoginCommand(
            context.Request.Username!,
            context.Request.Password!
        ));

        _logger.LogInformation("Login result: {@Result}", result);

        if (result is null)
        {
            context.Reject(
                error: OpenIddictConstants.Errors.InvalidGrant,
                description: "Invalid credentials.");
            return;
        }

        // Identity oluştur
        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Role);

        // Subject
        var sub = new Claim(OpenIddictConstants.Claims.Subject, result.UserId.ToString());
        sub.SetDestinations(OpenIddictConstants.Destinations.AccessToken,
                            OpenIddictConstants.Destinations.IdentityToken);
        identity.AddClaim(sub);

        // Name
        var name = new Claim(OpenIddictConstants.Claims.Name, result.UserName);
        name.SetDestinations(OpenIddictConstants.Destinations.AccessToken,
                             OpenIddictConstants.Destinations.IdentityToken);
        identity.AddClaim(name);

        // Email
        var email = new Claim(OpenIddictConstants.Claims.Email, result.Email);
        email.SetDestinations(OpenIddictConstants.Destinations.AccessToken,
                              OpenIddictConstants.Destinations.IdentityToken);
        identity.AddClaim(email);

        // Roles
        if (result.Roles != null && result.Roles.Any())
        {
            foreach (var role in result.Roles)
            {
                var roleClaim = new Claim(OpenIddictConstants.Claims.Role, role);
                roleClaim.SetDestinations(
                    OpenIddictConstants.Destinations.AccessToken,
                    OpenIddictConstants.Destinations.IdentityToken
                );
                identity.AddClaim(roleClaim);
            }
        }
        else
        {
            // ⚠️ TODO : Development için admin rolü eklendi
            var adminRole = new Claim(OpenIddictConstants.Claims.Role, "Admin");
            adminRole.SetDestinations(
                OpenIddictConstants.Destinations.AccessToken,
                OpenIddictConstants.Destinations.IdentityToken
            );
            identity.AddClaim(adminRole);
        }


        // Principal
        var principal = new ClaimsPrincipal(identity);

        // Scope ve resource (sabit ve güvenli)
        principal.SetScopes(OpenIddictConstants.Scopes.OpenId,
                            OpenIddictConstants.Scopes.Email,
                            OpenIddictConstants.Scopes.Profile,
                            OpenIddictConstants.Scopes.OfflineAccess,
                            "api");

        principal.SetResources("api");

        // OpenIddict’e teslim et—JWT üretimi burada yapılır
        context.SignIn(principal);
    }
}
