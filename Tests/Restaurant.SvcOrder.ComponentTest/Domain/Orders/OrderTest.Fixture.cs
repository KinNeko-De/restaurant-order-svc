using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Repositories;
using Restaurant.SvcOrder.Repositories.Orders;
using Restaurant.SvcOrder.Testing.Domain.Orders;
using Restaurant.SvcOrder.Testing.Repositories;
using Restaurant.SvcOrder.Testing.Repositories.Orders;

namespace Restaurant.SvcOrder.ComponentTest.Domain.Orders;


[TestFixture]
public partial class OrderTest
{
    private ComponentTestFixture Fixture { get; set; } = new ComponentTestFixture();

    [OneTimeSetUp]
    public void StartTest()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void EndTest()
    {
        Trace.Flush();
    }
    
    [TearDown]
    public async Task AfterEachTest()
    {
        await Fixture.CleanRepository();

    }

    private class ComponentTestFixture
    {
        public ComponentTestFixture()
        {
            OrderRepositoryFixture = new OrderRepositoryFixture(DatabaseConnectionProvider);
        }

        private DatabaseConnectionProvider DatabaseConnectionProvider { get; } = new DatabaseFixture().GetConnectionProviderToLocalDatabase();
        private OrderRepositoryFixture OrderRepositoryFixture { get; }

        public readonly Mock Mocks = new();
        public readonly TestData Data = new();


        public class Mock
        {
            public readonly ILogger<OrderRepository> LoggerRepository = new NullLogger<OrderRepository>();
        }

        public class TestData
        {
            public OrderBuilder CreateOrder()
            {
                return new OrderBuilder();
            }
        }


        public OrderRepository CreateRepositoryUnderTest()
        {
            return new OrderRepository(
                Mocks.LoggerRepository,
                DatabaseConnectionProvider,
                new OrderSourceEventMapping());
        }

        public async Task CleanRepository()
        {
            await OrderRepositoryFixture.CleanupTables();
        }
    }

}
