using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;

namespace Restaurant.SvcOrder.ComponentTest.Domain.Orders;

[NonParallelizable]
public partial class OrderPersistenceContextTest
{
    [Test]
    public void Load_ByOrderId_NotFound()
    {
        var id = OrderId.NewOrderId();
        var sut = CreateSystemUnderTest();

        var exception = Assert.ThrowsAsync<OrderNotFoundException>(() => sut.Load(id, CancellationToken.None));
        Assert.That(exception, Is.Not.Null);
        Assert.That(exception!.Message, Contains.Substring(id.ToString()));
    }

    /// <summary>
    /// Ensures that the source event <see cref="OrderCreated"/> can be saved and loaded.
    /// </summary>
    /// <returns></returns>
    [Test]
    public async Task SaveLoad_ByOrderId_OrderCreated()
    {
        var expectedOrderId = OrderId.NewOrderId();
        var expectedLastSequenceNumber = 1;

        var sut = CreateSystemUnderTest();
        var expectedOrder = CreateOrder()
            .WithId(expectedOrderId)
            .Build(sut);
        
        await expectedOrder.Save(CancellationToken.None);

        var actualOrder = await sut.Load(expectedOrderId, CancellationToken.None);

        Assert.That(expectedOrderId, Is.EqualTo(actualOrder.Id));
        Assert.That(expectedLastSequenceNumber, Is.EqualTo(actualOrder.GetLastSourceEventSequenceNumber()));
    }
}