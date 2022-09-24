using System.Transactions;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders;

namespace Restaurant.SvcOrder.Domain.Orders;

public class Order : AggregateRoot
{
    public Order(OrderCreated orderCreated)
    {
        var order = new Order();

        order.ApplySourceEvent(orderCreated);
        NewSourceEvent(orderCreated);
    }

    private Order() { }

    public OrderId Id { get; private set; }

    public void ApplySourceEvent(OrderCreated orderCreated)
    {
        Id =   orderCreated.OrderId;
    }

    public static async Task<Order> Load(OrderId orderId, IOrderRepository orderRepository, CancellationToken cancellationToken)
    {
        var order = new Order();

        var sourceEvents = await orderRepository.GetSourceEventsByOrderId(order, orderId, cancellationToken);

        if (!sourceEvents.Any()) { throw new OrderNotFoundException(orderId); }

        return order;
    }

    public async Task Save(IOrderRepository orderRepository, CancellationToken cancellationToken)
    {
        if (NeedsToBeSaved())
        {
            using var transactionScope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromSeconds(25), TransactionScopeAsyncFlowOption.Enabled);

            await orderRepository.SaveSourceEvents(Id, NotPersistedSourceEvents, cancellationToken);

            if (IsNew())
            {
                await orderRepository.SaveOrderRelation(this, cancellationToken);
            }
            else
            {
                await orderRepository.UpdateOrderRelation(this, cancellationToken);
            }

            transactionScope.Complete();

            Saved();
        }
    }
}