namespace Restaurant.SvcOrder.Domain.Orders;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(OrderId orderId) : base($"{nameof(Order)} with {nameof(OrderId)} '{orderId}' not found.")
    {
        
    }
}