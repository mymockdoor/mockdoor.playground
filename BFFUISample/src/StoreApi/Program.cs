using System.Text.Json;
using Authentication.Policies;
using Blazor6.Shared;
using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using StoreApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var configuration = builder.Configuration.GetSection("Configuration").Get<Configuration>();

builder.Services.Configure<Configuration>(builder.Configuration.GetSection("Configuration"));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();

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
        policy.RequireClaim("scope", "StoreApi");
    });
    
    options.AddPolicy("StoreUser", policy => policy.Requirements.Add(new PermissionRequirement("storeuser")));
    options.AddPolicy("StoreAdmin", policy => policy.Requirements.Add(new PermissionRequirement("storeadmin")));
});

var prodListJson = File.ReadAllText("Seed/ProductList.json");
var products = JsonSerializer.Deserialize<List<ProductItem>>(prodListJson);
builder.Services.AddSingleton(products);
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();