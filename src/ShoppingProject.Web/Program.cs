using ShoppingProject.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults()    // This sets up OpenTelemetry logging
       .AddLoggerConfigs();     // This adds Serilog for console formatting

using var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var startupLogger = loggerFactory.CreateLogger<Program>();

startupLogger.LogInformation("Starting web host");

builder.Services.AddOptionConfigs(builder.Configuration, startupLogger, builder);
builder.Services.AddServiceConfigs(startupLogger, builder);
builder.Services.AddControllers(); 

builder.Services.AddFastEndpoints()
                .SwaggerDocument(o =>
                {
                  o.ShortSchemaNames = true;
                });

var app = builder.Build();
app.MapControllers();
app.MapDefaultControllerRoute();
app.UseRouting(); 
app.UseAuthentication(); 
app.UseAuthorization(); 
app.UseCors();


await app.UseAppMiddlewareAndSeedDatabase();

app.Run();

public partial class Program { }
