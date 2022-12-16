using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;

namespace TestWebApi;

public class AuthService
{
    private readonly HttpClient _client;

    private static string _cachedStockApiAccessToken = null;
    private static string _cachedStoreApiAccessToken = null;
    private static string _cachedOrdersApiAccessToken = null;

    public AuthService(HttpClient client)
    {
        _client = client;
    }

    public async Task<string> GetAccessTokenForOrdersApiAsync()
    {
        if (_cachedOrdersApiAccessToken != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(_cachedOrdersApiAccessToken);

            // check token still valid, if so return valid cached token
            if (jwtSecurityToken.ValidTo >= DateTime.UtcNow.AddSeconds(10))
            {
                return _cachedOrdersApiAccessToken;
            }
        }
        
        Console.WriteLine("Renewing orders token...");
        var disco = await _client.GetDiscoveryDocumentAsync("https://localhost:5001");
        
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

        _cachedOrdersApiAccessToken = tokenResponse.AccessToken;
        return tokenResponse.AccessToken;
    }
    
    public async Task<string> GetAccessTokenForStockApiAsync()
    {
        if (_cachedStockApiAccessToken != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(_cachedStockApiAccessToken);

            // check token still valid, if so return valid cached token
            if (jwtSecurityToken.ValidTo >= DateTime.UtcNow.AddSeconds(10))
            {
                return _cachedStockApiAccessToken;
            }
        }
        
        Console.WriteLine("Renewing stock token...");
        var disco = await _client.GetDiscoveryDocumentAsync("https://localhost:5001");
        
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

        _cachedStockApiAccessToken = tokenResponse.AccessToken;
        return tokenResponse.AccessToken;
    }
    
    public async Task<string> GetAccessTokenForStoreApiAsync()
    {
        if (_cachedStoreApiAccessToken != null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(_cachedStoreApiAccessToken);

            // check token still valid, if so return valid cached token
            if (jwtSecurityToken.ValidTo >= DateTime.UtcNow.AddSeconds(10))
            {
                return _cachedStoreApiAccessToken;
            }
        }
        
        Console.WriteLine("Renewing store token...");
        var disco = await _client.GetDiscoveryDocumentAsync("https://localhost:5001");
        
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

        _cachedStoreApiAccessToken = tokenResponse.AccessToken;
        return tokenResponse.AccessToken;
    }
}