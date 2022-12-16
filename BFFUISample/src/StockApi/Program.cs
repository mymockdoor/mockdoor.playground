using System.Text.Json;
using Authentication.Policies;
using Blazor6.Shared;
using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using StockApi.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration.GetSection("Configuration").Get<Configuration>();
builder.Services.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = configuration.ServiceUrls.IdentityServiceUrl;
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "StockApi");
    });
    
    options.AddPolicy("StoreAdmin", policy => policy.Requirements.Add(new PermissionRequirement("storeadmin")));
});

var stockListJson = File.ReadAllText("Seed/StockList.json");
    
builder.Services.AddSingleton(JsonSerializer.Deserialize<List<StockItem>>(stockListJson));
builder.Services.AddScoped<StockService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();