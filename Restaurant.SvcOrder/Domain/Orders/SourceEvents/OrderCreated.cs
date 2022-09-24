using Google.Protobuf;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders.SourceEvents;

namespace Restaurant.SvcOrder.Domain.Orders.SourceEvents;

public class OrderCreated : ISourceEvent
{
    public SourceEventId Id { get; init; }

    public OrderId OrderId { get; init; }

    public IMessage ToDatamodel()
    {
        return OrderCreatedMapper.ToDatamodel(this);
    }
}
