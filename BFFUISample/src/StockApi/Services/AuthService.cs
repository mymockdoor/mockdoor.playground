using Blazor6.Shared.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace StockApi.Services;

public class AuthService
{
    private readonly HttpClient _client;
    private readonly Configuration _configuration;

    public AuthService(HttpClient client, IOptions<Configuration> options)
    {
        _client = client;
        _configuration = options.Value;
    }

    public async Task<string> GetAccessTokenForOrderProcessorApiAsync()
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

            ClientId = "OrderProcessorApiClient",
            ClientSecret = "secret",
            Scope = "OrderProcessorApi",
        });

        if (tokenResponse.IsError)
        {
            Console.WriteLine(tokenResponse.Error);
            Console.WriteLine(tokenResponse.ErrorDescription);
            return null;
        }

        return tokenResponse.AccessToken;
    }

    public async Task<string> GetAccessTokenForStoreApiAsync()
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

            ClientId = "StoreApiClient",
            ClientSecret = "secret",
            Scope = "StoreApi",
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