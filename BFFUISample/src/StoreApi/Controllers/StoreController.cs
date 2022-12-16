using Blazor6.Shared;
using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StoreApi.Services;

namespace StoreApi.Controllers;

[ApiController]
[Route("[controller]")]
public class StoreController : ControllerBase
{
    private readonly ILogger<StoreController> _logger;
    private readonly ProductService _storeService;

    public StoreController(ILogger<StoreController> logger, ProductService storeService)
    {
        _logger = logger;
        _storeService = storeService;
    }

    [HttpGet]
    [Authorize(Policy = "ApiScope")]
    public async Task<IEnumerable<ProductDto>> GetAll()
    {
        return await _storeService.GetAllAsync();
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "StoreUser")]
    public async Task<ProductItem> Get(int id)
    {
        return await _storeService.GetAsync(id);
    }

    [HttpPost("order")]
    [Authorize(Policy = "StoreUser")]
    public async Task<double> PlaceOrder([FromBody] ProductOrder order)
    {
        return await _storeService.PlaceOrderAsync(order);
    }
    
    [HttpPost("add")]
    [Authorize(Policy = "StoreAdmin")]
    public ActionResult AddProduct([FromBody] ProductItem productItem)
    {
        _storeService.Create(productItem);

        return Ok();
    }
}