using Blazor6.Shared.Models;

namespace OrderProcessorApi.Services;

public class OrdersService
{
    private static List<OrderProcessItem> _orders = new List<OrderProcessItem>();

    public void Add(OrderProcessItem item)
    {
        _orders.Add(item);
    }

    public List<OrderProcessItem> GetAll()
    {
        return _orders;
    }

    public List<OrderProcessItem> GetAllCurrentUser(int userId)
    {
        return _orders.Where(o => o.UserId == userId).ToList();
    }

    public OrderProcessItem ResetOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return null;

        order.ProcessStatus = ProcessStatus.NotStarted;

        return order;
    }

    public OrderProcessItem ProgressOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(o => o.OrderId == orderId);

        if (order == null)
            return null;
        
        order.ProcessStatus = GetNextProcessState(order.ProcessStatus);

        return order;
    }

    private ProcessStatus GetNextProcessState(ProcessStatus orderProcessStatus)
    {
        switch (orderProcessStatus)
        {
            case ProcessStatus.NotStarted: return ProcessStatus.Picking; 
            case ProcessStatus.Picking: return ProcessStatus.Packing; 
            case ProcessStatus.Packing: return ProcessStatus.Sent; 
            default: return ProcessStatus.Closed;
        }
    }

    public void DeleteOrderId(Guid orderId)
    {
        _orders.RemoveAll(o => o.OrderId == orderId);
    }
    
    public void DeleteAll()
    {
        _orders.Clear();
    }
}