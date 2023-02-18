using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Testing.Domain.Orders;

public class OrderBuilder
{
    private SourceEventId OrderCreatedSourceEventId { get; set; } = SourceEventId.NewSourceEventId();

    private OrderId Id { get; set; } = OrderId.NewOrderId();

    public OrderBuilder WithId(OrderId orderId)
    {
        Id = orderId;
        return this;
    }

    public Order Build(Order.PersistenceContext orderPersistenceContext)
    {
        var orderCreated = new OrderCreated()
        {
            Id = OrderCreatedSourceEventId,
            OrderId = Id
        };

        var testData = new Order(orderPersistenceContext, orderCreated);
        return testData;
    }
}