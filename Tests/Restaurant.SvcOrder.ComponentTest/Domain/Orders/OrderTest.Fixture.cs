using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Restaurant.SvcOrder.Repositories;
using Restaurant.SvcOrder.Repositories.Orders;
using Restaurant.SvcOrder.Testing.Domain.Orders;
using Restaurant.SvcOrder.Testing.Repositories;
using Restaurant.SvcOrder.Testing.Repositories.Orders;

namespace Restaurant.SvcOrder.ComponentTest.Domain.Orders;


[TestFixture]
public partial class OrderTest
{
    private readonly OrderRepositoryFixture orderRepositoryFixture = new (new DatabaseFixture().GetConnectionProviderToLocalDatabase());
    private Mocks mocks = new ();
    
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

    [SetUp]
    public Task BeforeEachTest()
    {
        mocks = new Mocks();
        return Task.CompletedTask;
    }
    
    [TearDown]
    public async Task AfterEachTest()
    {
        await CleanRepositories();
    }

    public async Task CleanRepositories()
    {
        await orderRepositoryFixture.CleanupTables();
    }

    /// <summary>
    /// Creates a order as test data.
    /// You can modify the test data using the builder pattern.
    /// </summary>
    /// <returns></returns>
    public OrderBuilder CreateOrder()
    {
        return new OrderBuilder();
    }

    private OrderRepository CreateRepositoryUnderTest()
    {
        return new OrderRepository(
            mocks.LoggerRepository,
            orderRepositoryFixture.DatabaseConnectionProvider,
            new OrderSourceEventMapping());
    }

    /// <summary>
    /// Defines all mocks that are used in this test.
    /// </summary>
    /// <remarks>Mocks can have state. Renew the instance before each test to avoid sharing state between tests.</remarks>
    private class Mocks
    {
        public readonly ILogger<OrderRepository> LoggerRepository = new NullLogger<OrderRepository>();
    }
}
