using Google.Protobuf;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.SourceEvents;

namespace Restaurant.SvcOrder.Repositories.Orders.SourceEvents;

/// <summary>

/// Converts the different persistence model to a common source event that is used inside of the domain.
/// </summary>
public class OrderCreatedMapper
{
    /// <summary>
    /// Used to persist newest source event
    /// For new source events always the current and newest persistence model is used to persist it.
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    public static OrderCreatedV2 ToDatamodel(OrderCreated domain)
    {
        return new OrderCreatedV2()
        {
            OrderId = Uuid.FromGuid(domain.OrderId.Guid)
        };
    }

    /// <summary>
    /// Converts the current persistence model to a common source event that is used inside of the domain.
    /// You may leave that out but that would pull your persistence model into your core domain
    /// </summary>
    /// <param name="order"></param>
    /// <param name="sourceEventId"></param>
    /// <param name="orderCreatedV2"></param>
    /// <returns></returns>
    public static OrderCreated FromDatamodel(Order order, SourceEventId sourceEventId, OrderCreatedV2 orderCreatedV2)
    {
        var domainSourceEvent = new OrderCreated()
        {
            Id = sourceEventId,
            OrderId = new OrderId(orderCreatedV2.OrderId.ToGuid())
        };

        order.ApplySourceEvent(domainSourceEvent);

        return domainSourceEvent;
    }

    /// <summary>
    /// Converts the an outdated persistence model to a common source event that is used inside of the domain.
    /// You do not need to convert the source events in the database
    /// </summary>
    /// <param name="order"></param>
    /// <param name="sourceEventId"></param>
    /// <param name="orderCreatedV1"></param>
    /// <returns></returns>
    public static OrderCreated FromDatamodel(Order order, SourceEventId sourceEventId, OrderCreatedV1 orderCreatedV1)
    {
        var orderGuid = Guid.Parse(orderCreatedV1.OrderId);
        var domainSourceEvent = new OrderCreated()
        {
            Id = sourceEventId,
            OrderId = new OrderId(orderGuid)
        };

        order.ApplySourceEvent(domainSourceEvent);

        return domainSourceEvent;
    }
}
