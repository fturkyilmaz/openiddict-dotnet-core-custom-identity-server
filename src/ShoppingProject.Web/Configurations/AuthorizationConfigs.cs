using Microsoft.AspNetCore.Authorization;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ShoppingProject.Web.Configurations;

public static class AuthorizationConfigs
{
    public static IServiceCollection AddAuthorizationConfigs(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // API access with specific scope and audience
            options.AddPolicy("ApiUsersRead", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api.users.read");
                policy.RequireClaim("aud", "shopping-api");
            });

            options.AddPolicy("ApiUsersWrite", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api.users.write");
                policy.RequireClaim("aud", "shopping-api");
            });

            options.AddPolicy("ApiAdmin", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("scope", "api.admin");
                policy.RequireClaim("aud", "shopping-admin");
            });

            // Fallback policy for any authenticated user with basic scopes
            options.AddPolicy("ApiBasic", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var hasScope = context.User.HasClaim(c => c.Type == "scope" && 
                        (c.Value == "clientapi" || c.Value == "postman"));
                    var hasAudience = context.User.HasClaim("aud", "api");
                    return hasScope && hasAudience;
                });
            });

            // Permission-based policies
            options.AddPolicy("UsersRead", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("permission", "users.read");
            });

            options.AddPolicy("UsersManage", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("permission", "users.manage");
            });

            options.AddPolicy("UsersWrite", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("permission", "users.write");
            });
        });

        return services;
    }
}