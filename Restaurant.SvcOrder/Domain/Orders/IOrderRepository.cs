using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Domain.Orders;

public interface IOrderRepository
{
    Task<IReadOnlyCollection<ISourceEvent>> GetSourceEventsByOrderId(Order order, OrderId orderId, CancellationToken cancellationToken);

    Task SaveSourceEvents(OrderId orderId, IDictionary<int, ISourceEvent> addedSourceEvents, CancellationToken cancellationToken);

    Task SaveOrderRelation(Order order, CancellationToken cancellationToken);

    Task UpdateOrderRelation(Order order, CancellationToken cancellationToken);
}