using System.Transactions;

namespace Restaurant.SvcOrder.Domain.Orders;

public partial class Order
{
    private readonly PersistenceContext persistenceContext;

    /// <summary>
    /// Creates an instance of the aggregate without applying any source event to it
    /// </summary>
    /// <param name="persistenceContext"></param>
    private Order(PersistenceContext persistenceContext)
    {
        this.persistenceContext = persistenceContext;
    }

    private static async Task<Order> Load(OrderId orderId, PersistenceContext persistenceContext, CancellationToken cancellationToken)
    {
        var order = new Order(persistenceContext);

        var sourceEvents = await persistenceContext.OrderRepository.GetSourceEventsByOrderId(order, orderId, cancellationToken);

        if (!sourceEvents.Any()) { throw new OrderNotFoundException(orderId); }

        return order;
    }

    public async Task Save(CancellationToken cancellationToken)
    {
        await persistenceContext.Save(this, cancellationToken);
    }

    private async Task Save(IOrderRepository orderRepository, CancellationToken cancellationToken)
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

    /// <summary>
    /// Handles load and save of the aggregate root.
    /// It also can save business events to ensure transactional security.
    /// Pulls every dependency that is needed for this.
    /// It pulls the dependency using dependency injection.
    /// </summary>
    public class PersistenceContext
    {
        public readonly IOrderRepository OrderRepository;

        public PersistenceContext(IOrderRepository orderRepository)
        {
            this.OrderRepository = orderRepository;
        }

        public async Task<Order> Load(OrderId orderId, CancellationToken cancellationToken)
        {
            return await Order.Load(orderId, this, cancellationToken);
        }

        public async Task Save(Order unsavedOrder, CancellationToken cancellationToken)
        {
            await unsavedOrder.Save(OrderRepository, cancellationToken);
        }
    }
}
