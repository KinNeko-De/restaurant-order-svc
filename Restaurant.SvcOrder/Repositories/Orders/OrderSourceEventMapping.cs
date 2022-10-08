using System.Collections.Concurrent;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders.SourceEvents;

namespace Restaurant.SvcOrder.Repositories.Orders;

/// <summary>
/// Dependency injection singleton
/// </summary>
public class OrderSourceEventMapping : SourceEventMapping<Order>
{
    public OrderSourceEventMapping()
    {
        Mappers.TryAdd(OrderCreatedV1.Descriptor.FullName, 
            (OrderCreatedV1.Descriptor,
                (order, sourceEventId, message) => OrderCreatedMapper.FromDatamodel(order, sourceEventId, (OrderCreatedV1)message)));
        Mappers.TryAdd(OrderCreatedV2.Descriptor.FullName,
            (OrderCreatedV2.Descriptor, 
                (order, sourceEventId, message) => OrderCreatedMapper.FromDatamodel(order, sourceEventId, (OrderCreatedV2)message)));
    }
}
