using System.Globalization;
using DataDash.Models;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Models;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StoreMonitorController : ControllerBase
    {

        private readonly HttpClient _client;
        private readonly AuthService _authService;
        private readonly Configuration _configuration;

        public StoreMonitorController(HttpClient client, AuthService authService, IOptions<Configuration> configurationOptions)
        {
            _client = client;
            _authService = authService;
            _configuration = configurationOptions.Value;
        }

        [HttpGet]
        public async Task<DataTransferObject> Get()
        {
            DataTransferObject dashboardData = new DataTransferObject();
            dashboardData.NumberOfColumns = 12;
            dashboardData.NumberOfRows = 10;

            await AddProductItemsPanel(dashboardData);
            
            await AddStockItemsPanel(dashboardData);

            await AddOrderProcessingPanel(dashboardData);

            return dashboardData;
        }
        
        private async Task AddProductItemsPanel(DataTransferObject dashboardData)
        {
            _client.SetBearerToken(await _authService.GetAccessTokenForStoreApiAsync());

            var stocksResponse = await _client.GetAsync($"{_configuration.ServiceUrls.StoreServiceUrl}/store");

            var products = new List<ProductDto>();
            if (stocksResponse.IsSuccessStatusCode)
            {
                var productsContent = await stocksResponse.Content.ReadFromJsonAsync<List<ProductDto>>();

                if (productsContent?.Count > 0)
                {
                    products.AddRange(productsContent);
                }
            }

            dashboardData.TitleRows.Add(new TitleRow()
            {
                TopLeftColumnNumber = 0,
                TopLeftRowNumber = 0,
                BottomRightColumnNumber = 3,
                BottomRightRowNumber = 0,
                Title = new TextElement("Product watcher")
            });

            dashboardData.DisplayTables.Add(new DisplayTable()
            {
                TopLeftColumnNumber = 0,
                TopLeftRowNumber = 1,
                BottomRightColumnNumber = 3,
                BottomRightRowNumber = 9,
                headers = new TextElement[]
                {
                    new TextElement("Product Id"),
                    new TextElement("Product Name"),
                    new TextElement("Product Price")
                },
                dataRows = products.Select(p => new TextElement[]
                    {
                        new TextElement(p.Id.ToString()),
                        new TextElement(p.Name),
                        new TextElement(p.Price.ToString("£0.00"))
                    })
                    .ToList()
            });
        }
        
        private async Task AddStockItemsPanel(DataTransferObject dashboardData)
        {
            _client.SetBearerToken(await _authService.GetAccessTokenForStockApiAsync());

            var stocksResponse = await _client.GetAsync($"{_configuration.ServiceUrls.StockServiceUrl}/stock");

            var stocks = new List<StockItem>();
            if (stocksResponse.IsSuccessStatusCode)
            {
                var stocksContent = await stocksResponse.Content.ReadFromJsonAsync<List<StockItem>>();

                if (stocksContent?.Count > 0)
                {
                    stocks.AddRange(stocksContent);
                }
            }

            dashboardData.TitleRows.Add(new TitleRow()
            {
                TopLeftColumnNumber = 4,
                TopLeftRowNumber = 0,
                BottomRightColumnNumber = 5,
                BottomRightRowNumber = 0,
                Title = new TextElement("Stock watcher")
            });

            dashboardData.DisplayTables.Add(new DisplayTable()
            {
                TopLeftColumnNumber = 4,
                TopLeftRowNumber = 1,
                BottomRightColumnNumber = 5,
                BottomRightRowNumber = 9,
                headers = new TextElement[]
                {
                    new TextElement("Stock/Product Id"),
                    new TextElement("Stock Level")
                },
                dataRows = stocks.Select(s => new TextElement[]
                    {
                        new TextElement(s.ProductId.ToString()),
                        new TextElement(s.StockLeft.ToString(),
                            s.StockLeft > 10 ? "green" : s.StockLeft > 5 ? "default" : "red")
                    })
                    .ToList()
            });
        }

        private async Task AddOrderProcessingPanel(DataTransferObject dashboardData)
        {
            _client.SetBearerToken(await _authService.GetAccessTokenForOrdersApiAsync());

            var ordersResponse = await _client.GetAsync($"{_configuration.ServiceUrls.OrderProcessorServiceUrl}/orderprocessor");
            var orders = new List<OrderProcessItem>();
            if (ordersResponse.IsSuccessStatusCode)
            {
                var ordersContent = await ordersResponse.Content.ReadFromJsonAsync<List<OrderProcessItem>>();

                if (ordersContent?.Count > 0)
                {
                    orders.AddRange(ordersContent);
                }
            }

            dashboardData.TitleRows.Add(new TitleRow()
            {
                TopLeftColumnNumber = 6,
                TopLeftRowNumber = 0,
                BottomRightColumnNumber = 11,
                BottomRightRowNumber = 0,
                Title = new TextElement("Order watcher")
            });

            dashboardData.DisplayTables.Add(new DisplayTable()
            {
                TopLeftColumnNumber = 6,
                TopLeftRowNumber = 1,
                BottomRightColumnNumber = 11,
                BottomRightRowNumber = 9,
                headers = new TextElement[]
                {
                    new TextElement("Order Id"),
                    new TextElement("Product Id"),
                    new TextElement("Status"),
                    new TextElement("Price"),
                    new TextElement("Quantity"),
                    new TextElement("Total price"),
                    new TextElement("User"),
                    new TextElement("Placed at"),
                    new TextElement("Due date")
                },
                dataRows = orders.Select(oi => new TextElement[]
                    {
                        new TextElement(oi.OrderId.ToString()),
                        new TextElement(oi.ProductId.ToString()),
                        new TextElement(oi.ProcessStatus.ToString()),
                        new TextElement($"£{oi.Price}"),
                        new TextElement(oi.Quantity.ToString()),
                        new TextElement($"£{(oi.Quantity * oi.Price).ToString("0.00")}"),
                        new TextElement(oi.UserId.ToString()),
                        new TextElement(oi.OrderPlacedAt.ToString(CultureInfo.InvariantCulture)),
                        new TextElement(oi.DueDate.ToString(CultureInfo.InvariantCulture))
                    })
                    .ToList()
            });
        }
    }
}