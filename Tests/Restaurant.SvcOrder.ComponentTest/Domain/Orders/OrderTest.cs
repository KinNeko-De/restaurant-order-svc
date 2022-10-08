using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Testing.Domain.Orders;

namespace Restaurant.SvcOrder.ComponentTest.Domain.Orders;

[NonParallelizable]
public partial class OrderTest
{
    [Test]
    public void Load_ByOrderId_NotFound()
    {
        var id = OrderId.NewOrderId();
        var repositoryUnderTest = Fixture.CreateRepositoryUnderTest();

        var exception = Assert.ThrowsAsync<OrderNotFoundException>(() => Order.Load(id, repositoryUnderTest, CancellationToken.None));
        Assert.NotNull(exception);
        StringAssert.Contains(id.ToString(), exception!.Message);
    }

    /// <summary>
    /// Ensures that the source event <see cref="OrderCreated"/> can be saved and loaded.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task SaveLoad_ByOrderId_OrderCreated()
    {
        var expectedOrderId = OrderId.NewOrderId();

        var expectedOrder = Fixture.Data.CreateOrder()
            .WithId(expectedOrderId)
            .Build();
        
        var repositoryUnderTest = Fixture.CreateRepositoryUnderTest();
        await expectedOrder.Save(repositoryUnderTest, CancellationToken.None);

        var actualOrder = await Order.Load(expectedOrderId, repositoryUnderTest, CancellationToken.None);

        Assert.AreEqual(expectedOrderId, actualOrder.Id);
    }
}