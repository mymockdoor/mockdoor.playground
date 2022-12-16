using Blazor6.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessorApi.Services;

namespace OrderProcessorApi.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Policy = "OrderProcessorApiScope")]
public class OrderProcessorController : ControllerBase
{
    private readonly ILogger<OrderProcessorController> _logger;
    private readonly OrdersService _ordersService;

    public OrderProcessorController(ILogger<OrderProcessorController> logger, OrdersService ordersService)
    {
        _logger = logger;
        _ordersService = ordersService;
    }

    [HttpGet]
    public List<OrderProcessItem> GetAll()
    {
        return _ordersService.GetAll();
    }
    
    [HttpGet("allcurrentuser")]
    
    public List<OrderProcessItem> GetAllCurrentUser()
    {
        var userId = HttpContext.User.Claims.First(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")).Value;
        return _ordersService.GetAllCurrentUser(int.Parse(userId));
    }

    [HttpPost("add")]
    public ActionResult Add([FromBody] OrderProcessItem item)
    {
        if (item == null)
            return BadRequest();

        _ordersService.Add(item);
        
        return Ok();
    }

    [HttpPost("resetorder/{orderId}")]
    public ActionResult<OrderProcessItem> ResetOrder(Guid orderId)
    {
        var order = _ordersService.ResetOrder(orderId);

        if (order == null)
            return BadRequest();

        return Ok(order);
    }

    [HttpPost("progressorder/{orderId}")]
    public ActionResult<OrderProcessItem> ProgressOrder(Guid orderId)
    {
        var order = _ordersService.ProgressOrder(orderId);

        if (order == null)
            return BadRequest();

        return Ok(order);
    }

    [HttpDelete("{orderId}")]
    public ActionResult DeleteById(Guid orderId)
    {
        _ordersService.DeleteOrderId(orderId);
        return Ok();
    }

    [HttpDelete("all")]
    public ActionResult DeleteAll()
    {
        _ordersService.DeleteAll();
        return Ok();
    }
}