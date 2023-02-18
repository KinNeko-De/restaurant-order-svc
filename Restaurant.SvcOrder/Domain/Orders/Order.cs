using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Domain.Orders;

/// <summary>
/// Domain part of the aggregate root
/// </summary>
public partial class Order : AggregateRoot
{
    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="persistenceContext">The context that handles the persistence.</param>
    /// <param name="orderCreated">Initial source event to create the aggregate root.</param>
    public Order(PersistenceContext persistenceContext, OrderCreated orderCreated) : this(persistenceContext)
    {
        ApplySourceEvent(orderCreated);
        NewSourceEvent(orderCreated);
    }

    public OrderId Id { get; private set; }

    public void ApplySourceEvent(OrderCreated orderCreated)
    {
        Id =  orderCreated.OrderId;
        SourceEventApplied(orderCreated);
    }
}