using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StockApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StockController : ControllerBase
{
    private readonly ILogger<StockController> _logger;
    private readonly Services.StockService _stockService;

    public StockController(ILogger<StockController> logger, Services.StockService stockService)
    {
        _logger = logger;
        _stockService = stockService;
    }

    [HttpGet]
    [Authorize(Policy = "ApiScope")]
    public IEnumerable<StockItem> GetAll()
    {
        return _stockService.GetAll();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "ApiScope")]
    public StockItem Get(int id)
    {
        return _stockService.Get(id);
    }

    [HttpPost("order")]
    [Authorize(Policy = "ApiScope")]
    public async Task<int> PlaceOrder([FromBody] ProductOrder order)
    {
        return await _stockService.PlaceOrder(order, HttpContext);
    }

    [HttpPost("setstock")]
    [Authorize(Policy = "StoreAdmin")]
    public ActionResult SetStock([FromBody] StockItem stockUpdate)
    {
        if (_stockService.SetStock(stockUpdate))
        {
            return StatusCode(201);
        }

        return Ok();
    }
}