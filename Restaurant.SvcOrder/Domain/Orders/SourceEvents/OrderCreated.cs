using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Domain.Orders.SourceEvents;

public class OrderCreated : ISourceEvent
{
    public SourceEventId Id { get; init; }

    public OrderId OrderId { get; init; }
}
