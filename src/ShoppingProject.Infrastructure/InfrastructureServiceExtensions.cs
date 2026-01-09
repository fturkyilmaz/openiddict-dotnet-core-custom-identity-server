using ShoppingProject.Core.Interfaces;
using ShoppingProject.Core.Services;
using ShoppingProject.Infrastructure.Data;
using ShoppingProject.Infrastructure.Data.Queries;
using ShoppingProject.UseCases.Contributors.List;
using ShoppingProject.Core.UserAggregate;
using ShoppingProject.UseCases.Users.Interfaces;
using ShoppingProject.Infrastructure.Auth;

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
           .AddScoped<IDeleteContributorService, DeleteContributorService>();
    
    services.AddScoped<ITokenService, TokenService>(); 
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IRevokeTokenService, RevokeTokenService>();

    services.AddOpenIddict()
    .AddCore(options => { options.UseEntityFrameworkCore() .UseDbContext<AppDbContext>(); })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("/auth/token");
        options.SetRevocationEndpointUris("/auth/revoke");
        options.AllowPasswordFlow();
        options.AllowRefreshTokenFlow();
        options.AllowClientCredentialsFlow();

        options.AcceptAnonymousClients();

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.SetIssuer("ShoppingProject");


        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .DisableTransportSecurityRequirement(); // dev environment
        
        options.RegisterScopes("api", "openid", "profile");
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
