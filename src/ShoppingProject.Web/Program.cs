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


var app = builder.Build();
app.MapControllers();
app.MapDefaultControllerRoute();
app.UseRouting(); 
app.UseAuthentication(); 
app.UseAuthorization(); 
app.UseCors();

// if (app.Environment.IsDevelopment())
//   {
//       app.UseSwagger();
//       app.UseSwaggerUI(c =>
//       {
//           c.SwaggerEndpoint("/openapi/v1.json", "ShoppingProject API v1");
//           c.RoutePrefix = string.Empty;
//       });

//       // Scalar UI
//       app.MapScalarApiReference();
//   }


await app.UseAppMiddlewareAndSeedDatabase();

app.Run();

public partial class Program { }
