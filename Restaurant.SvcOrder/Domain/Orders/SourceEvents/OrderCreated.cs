using Google.Protobuf;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders.SourceEvents;

namespace Restaurant.SvcOrder.Domain.Orders.SourceEvents;

/// <summary>
/// Domain source event because:
/// 1. data types: There is no difference between a Guid and a Guid? in protobuf. also there is no difference between dateonly and datetime
/// 2. test data based on c# types
/// 3. common event for breaking changes. alternatively you can always use the highest protobuf source event and convert all other to that
/// </summary>
public class OrderCreated : ISourceEvent
{
    public SourceEventId Id { get; init; }

    public OrderId OrderId { get; init; }

    public IMessage ToDatamodel()
    {
        return OrderCreatedMapper.ToDatamodel(this);
    }
}
