using IdentityServer;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.Configure<IdentityConfiguration>(builder.Configuration.GetSection("Authentication"));
    builder.Services.AddScoped<UserStoreAuthenticationService>();
    builder.Services.AddHttpClient();

    IdentityConfiguration identityConfiguration = new IdentityConfiguration();
    builder.Configuration.Bind("Authentication", identityConfiguration);
    
    var app = builder
        .ConfigureServices()
        .ConfigurePipeline(identityConfiguration);
    
    app.Run();
}
catch (Exception ex)
{
    if (ex.GetType().Name != "StopTheHostException")
    {
        Log.Fatal(ex, "Unhandled exception");
    }
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();
}