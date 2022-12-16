using System.Text;
using System.Text.Json;
using IdentityServer.Models;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services;

public class UserStoreAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IdentityConfiguration _identityConfiguration;

    public UserStoreAuthenticationService(HttpClient httpClient, IOptions<IdentityConfiguration> identityConfiguration)
    {
        _httpClient = httpClient;
        _identityConfiguration = identityConfiguration?.Value ?? throw new ArgumentNullException(nameof(identityConfiguration));
    }
    
    public async Task<TestUser> AuthenticateUserAsync(string username, string password, string provider)
    {
        var authenticateRequest = new AuthenticateDto() { Username = username, Password = password, ProviderName = provider };
        var result = await _httpClient.PostAsync($"{_identityConfiguration.UserStoreUrl}/user/authenticate", new StringContent(JsonSerializer.Serialize(authenticateRequest), Encoding.Default, "application/json"));

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<TestUser>();
        }

        return null;
    }

    public async Task<TestUser> FindByExternalProviderAsync(string provider, string providerUserId)
    {
        var result = await _httpClient.GetAsync($"{_identityConfiguration.UserStoreUrl}/user/findbyprovider/{provider}/{providerUserId}");

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<TestUser>();
        }

        return null;
    }

    public async Task<TestUser> AutoProvisionUserAsync(string provider, string providerUserId, List<Claim> claims)
    {
      
        var authenticateRequest = new ProvisionUserDto() { ProviderName = provider, ProviderSubjectId = providerUserId, Claims = claims };
        var result = await _httpClient.PostAsync($"{_identityConfiguration.UserStoreUrl}/user/provision", new StringContent(JsonSerializer.Serialize(authenticateRequest), Encoding.Default, "application/json"));

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<TestUser>();
        }

        return null;
    }
}