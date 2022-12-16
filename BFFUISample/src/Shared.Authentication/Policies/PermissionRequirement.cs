using System.Net.Http.Json;
using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Authentication.Policies;

public class PermissionRequirement : IAuthorizationRequirement
{
    public string RequiredPermission { get; set; }

    public PermissionRequirement(string requiredPermission)
    {
        RequiredPermission = requiredPermission;
    }
}

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly HttpClient _httpClient;
    private readonly Configuration _configuration;

    public PermissionRequirementHandler(HttpClient httpClient, IOptions<Configuration> configurationOptions)
    {
        _httpClient = httpClient;
        _configuration = configurationOptions.Value;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userPermissions = context.User.Claims.FirstOrDefault(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"));

        if (userPermissions?.Value == null)
        {
            context.Fail();
            return;
        }
        
        var permissionsResponse = await _httpClient.GetAsync(
            $"{_configuration.ServiceUrls.PermissionServiceUrl}/permissions/{userPermissions.Value}");

        if (!permissionsResponse.IsSuccessStatusCode)
        {
            context.Fail();
            return;
        }

        var permissions = await permissionsResponse.Content.ReadFromJsonAsync<List<string>>();

        if (permissions == null || !permissions.Contains(requirement.RequiredPermission))
        {
            context.Fail();
            return;
        }
        
        context.Succeed(requirement);
    }
}