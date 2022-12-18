using System.Text;
using Blazor6.Shared.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace StockApi.Services;

public class StockService
{
    private readonly AuthService _authService;
    private readonly HttpClient _client;
    private List<StockItem> _stockItems;
    private readonly Configuration _configuration;

    public StockService(List<StockItem> stockItems, AuthService authService, HttpClient client, IOptions<Configuration> options)
    {
        _authService = authService;
        _client = client;
        _stockItems = stockItems ?? new List<StockItem>();
        _configuration = options.Value;
    }

    public StockItem Get(int id)
    {
        return _stockItems.FirstOrDefault(p => p.ProductId == id);
    }

    public List<StockItem> GetAll()
    {
        return _stockItems;
    }

    public void Create(StockItem stockItem)
    {
        _stockItems.Add(stockItem);
    }

    public void Delete(int id)
    {
        _stockItems.RemoveAll(p => p.ProductId == id);
    }

    public async Task<int> PlaceOrder(ProductOrder order, HttpContext context)
    {
        var stockItem = Get(order.ProductId);
        stockItem.StockLeft -= order.Quantity;
        
        
        _client.SetBearerToken(await _authService.GetAccessTokenForStoreApiAsync());
        var productsListResponse = await _client.GetAsync($"{_configuration.ServiceUrls.StoreServiceUrl}/store");

        if (!productsListResponse.IsSuccessStatusCode)
            return -1;

        var productList = await productsListResponse.Content.ReadFromJsonAsync<List<ProductDto>>();

        var product = productList.FirstOrDefault(p => p.Id == order.ProductId);

        if (product == null)
            return -1;
        
        var content = new StringContent(JsonConvert.SerializeObject(new OrderProcessItem()
        {
            OrderId = Guid.NewGuid(),
            ProductId =order.ProductId,
            Quantity = order.Quantity,
            Price = product.Price,
            UserId = order.UserId,
            OrderPlacedAt = DateTime.Now,
            ProcessStatus = ProcessStatus.Picking,
            DueDate = DateTime.Now.AddDays(7)
        }), Encoding.Default, "application/json");
            
        _client.SetBearerToken(await _authService.GetAccessTokenForOrderProcessorApiAsync());
        await _client.PostAsync($"{_configuration.ServiceUrls.OrderProcessorServiceUrl}/orderprocessor/add", content);
        
        return stockItem.StockLeft;
    }

    public bool SetStock(StockItem stockUpdate)
    {
        var existingItem = Get(stockUpdate.ProductId);

        if (existingItem == null)
        {
            Create(stockUpdate);

            return true;
        }
        
        existingItem.StockLeft = stockUpdate.StockLeft;
        return false;
    }
}