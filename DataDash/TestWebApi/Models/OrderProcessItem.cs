namespace TestWebApi.Models;

public class OrderProcessItem 
{
    public Guid OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
    
    public double Price { get; set; }
    
    public ProcessStatus ProcessStatus { get; set; }

    public DateTime OrderPlacedAt { get; set; }

    public DateTime DueDate { get; set; }

    public int UserId { get; set; }
}

public enum ProcessStatus
{
    NotStarted,
    Picking,
    Packing,
    Sent,
    Closed
}