using Restaurant.SvcOrder.Domain.Orders;

namespace Restaurant.SvcOrder.Repositories;

public class OptimisticLockingException : Exception
{
    public OptimisticLockingException(OrderId orderId) : base($"{nameof(Order)} with {nameof(OrderId)} '{orderId}' was updated concurrently. Retry your operation.") {
    }
}