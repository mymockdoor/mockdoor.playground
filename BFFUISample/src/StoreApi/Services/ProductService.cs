using Blazor6.Shared;
using Blazor6.Shared.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace StoreApi.Services;

public class ProductService
{
    private readonly HttpClient _client;
    private readonly AuthService _authService;
    private List<ProductItem> _products = new();
    private readonly Configuration _configuration;

    public ProductService(List<ProductItem> products, HttpClient client, IOptions<Configuration> options, AuthService authService)
    {
        _client = client;
        _authService = authService;
        _configuration = options.Value;
        _products = products ?? new List<ProductItem>();
    }

    public async Task<ProductDto> GetAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);

        if (product == null)
            return null;
        _client.SetBearerToken(await _authService.GetAccessTokenForStockApiAsync());
        
        var stockResponse = await _client.GetAsync($"{_configuration.ServiceUrls.StockServiceUrl}/stock/{id}");

        if (stockResponse.IsSuccessStatusCode)
        {
            var stockItem = await stockResponse.Content.ReadFromJsonAsync<StockItem>();
            return  new ProductDto()
            {
                Id = product.Id,
                Price = product.Price,
                Description = product.Description,
                Name = product.Name,
                Stock = stockItem?.StockLeft ?? 0,
                ImageUrl = product.ImageUrl
            };
        }
        return null;
    }

    public async  Task<List<ProductDto>> GetAllAsync()
    {
        _client.SetBearerToken(await _authService.GetAccessTokenForStockApiAsync());
        var stockResponse = await _client.GetAsync($"{_configuration.ServiceUrls.StockServiceUrl}/stock");

        if (stockResponse.IsSuccessStatusCode)
        {
            var stockItems = await stockResponse.Content.ReadFromJsonAsync<List<StockItem>>();
            return _products.Select(p => new ProductDto()
            {
                Id = p.Id,
                Price = p.Price,
                Description = p.Description,
                Name = p.Name,
                Stock = stockItems.FirstOrDefault(si => si.ProductId == p.Id)?.StockLeft ?? 0,
                ImageUrl = p.ImageUrl
            }).ToList();
        }
        
        return null;
    }

    public void Create(ProductItem productItem)
    {
        _products.Add(productItem);
    }

    public void Delete(int id)
    {
        _products.RemoveAll(p => p.Id == id);
    }

    public async Task<double> PlaceOrderAsync(ProductOrder order)
    {
        var product = await GetAsync(order.ProductId);
        
        _client.SetBearerToken(await _authService.GetAccessTokenForStockApiAsync());
        await _client.PostAsJsonAsync($"{_configuration.ServiceUrls.StockServiceUrl}/stock/order", order);

        return product.Price * order.Quantity;
    }
}