using DataDash.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using TestWebApi.Models;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly HttpClient _client;
        private readonly AuthService _authService;

        public TestController(HttpClient client, AuthService authService)
        {
            _client = client;
            _authService = authService;
        }
        

        [HttpGet(Name = "GetTest")]
        public async Task<DataTransferObject> GetAsync()
        {
            _client.SetBearerToken(await _authService.GetAccessTokenForStockApiAsync());

            var stocksResponse = await _client.GetAsync("https://localhost:7089/stock");

            var stocks = new List<StockItem>();
            if (stocksResponse.IsSuccessStatusCode)
            {
                var stocksContent = await stocksResponse.Content.ReadFromJsonAsync<List<StockItem>>();

                if (stocksContent?.Count > 0)
                {
                    stocks.AddRange(stocksContent);
                }
            }
            
            
            DataTransferObject dashboardData = new DataTransferObject();
            dashboardData.NumberOfColumns = 8;
            dashboardData.NumberOfRows = 8;

            dashboardData.TitleRows.Add(new TitleRow()
            {
                TopLeftColumnNumber = 0,
                TopLeftRowNumber = 0,
                BottomRightColumnNumber = 8,
                BottomRightRowNumber = 0,
                Title = new TextElement("Stock Item panel")
            });


            dashboardData.DisplayTables.Add(new DisplayTable()
            {
                TopLeftColumnNumber = 0,
                TopLeftRowNumber = 4,
                BottomRightColumnNumber = 7,
                BottomRightRowNumber = 8,
                headers = new TextElement[] {
                      new TextElement("Stock/Product Id"),
                      new TextElement("Stock Level")
                   },
                dataRows = stocks.Select(s => new TextElement[]
                    {
                        new TextElement(s.ProductId.ToString()), 
                        new TextElement(s.StockLeft.ToString())
                    })
                    .ToList()
            });

            dashboardData.TitleRows.Add(new TitleRow()
            {
                TopLeftColumnNumber = 1,
                TopLeftRowNumber = 3,
                BottomRightColumnNumber = 8,
                BottomRightRowNumber = 3,
                Title = new TextElement("Title Text")
            });

            _client.SetBearerToken(await _authService.GetAccessTokenForOrdersApiAsync());

            var ordersResponse = await _client.GetAsync("https://localhost:7054/orderprocessor");
            
            
            dashboardData.HostingInfoFooter = new HostingInfoFooter(11, 11);

            return dashboardData;
        }
    }
}