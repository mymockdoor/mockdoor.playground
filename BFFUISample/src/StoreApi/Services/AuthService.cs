using Blazor6.Shared.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace StoreApi.Services;

public class AuthService
{
    private readonly HttpClient _client;
    private readonly Configuration _configuration;

    public AuthService(HttpClient client, IOptions<Configuration> options)
    {
        _client = client;
        _configuration = options.Value;
    }

    public async Task<string> GetAccessTokenForStockApiAsync()
    {
        var disco = await _client.GetDiscoveryDocumentAsync(_configuration.ServiceUrls.IdentityServiceUrl);
        
        if (disco.IsError)
        {
            Console.WriteLine(disco.Error);
            return null;
        }
        
        var tokenResponse = await _client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,

            ClientId = "StockApiClient",
            ClientSecret = "secret",
            Scope = "StockApi",
        });

        if (tokenResponse.IsError)
        {
            Console.WriteLine(tokenResponse.Error);
            Console.WriteLine(tokenResponse.ErrorDescription);
            return null;
        }

        return tokenResponse.AccessToken;
    }
}