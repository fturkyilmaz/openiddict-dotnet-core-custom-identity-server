using ShoppingProject.Core.Interfaces;
using ShoppingProject.Core.Services;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Infrastructure.Data.Queries;
using ShoppingProject.UseCases.Contributors.List;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.Infrastructure.Auth;
using OpenIddict.Server;
using OpenIddict.Abstractions;

namespace ShoppingProject.Infrastructure;
public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    string? connectionString =  config.GetConnectionString("SqliteConnection");
    Guard.Against.Null(connectionString);

    services.AddScoped<EventDispatchInterceptor>();
    services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();

    services.AddDbContext<AppDbContext>((provider, options) =>
    {
      var eventDispatchInterceptor = provider.GetRequiredService<EventDispatchInterceptor>();
      
      options.UseSqlite(connectionString);

      options.UseOpenIddict();
      
      options.AddInterceptors(eventDispatchInterceptor);
    });

    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>))
            .AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>))
            .AddScoped<IListContributorsQueryService, ListContributorsQueryService>()
            .AddScoped<IDeleteContributorService, DeleteContributorService>()

            .AddScoped<ITokenService, TokenService>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<IRevokeTokenService, RevokeTokenService>();


            services.AddHttpContextAccessor(); 
            services.AddScoped<ICurrentUserService, CurrentUserService>();

    services.AddOpenIddict()
    .AddCore(options => { options.UseEntityFrameworkCore() .UseDbContext<AppDbContext>(); })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("connect/token")
               .SetRevocationEndpointUris("connect/revoke")
               .SetAuthorizationEndpointUris("connect/authorize");

        options.AllowPasswordFlow()
               .AllowRefreshTokenFlow()
               .AllowClientCredentialsFlow();


        options.AcceptAnonymousClients();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.SetIssuer("https://localhost:57679");


        options.UseAspNetCore()
              .EnableAuthorizationEndpointPassthrough();
        
       options.RegisterScopes(OpenIddictConstants.Scopes.OpenId, OpenIddictConstants.Scopes.Profile, OpenIddictConstants.Scopes.OfflineAccess);

        options.AddEventHandler<OpenIddictServerEvents.HandleTokenRequestContext>(
          builder => builder.UseScopedHandler<PasswordGrantHandler>()
        );
    })
    .AddValidation(options =>
    {
        // Token doğrulama için local server 
        options.UseLocalServer();
        options.UseAspNetCore();
    });

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
