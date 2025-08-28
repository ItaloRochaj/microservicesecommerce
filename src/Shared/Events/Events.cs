namespace Shared.Events;

public class StockUpdateEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Operation { get; set; } = string.Empty; // "decrease" ou "increase"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string OrderId { get; set; } = string.Empty;
}

public class OrderCreatedEvent
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<OrderItemEvent> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class OrderItemEvent
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
