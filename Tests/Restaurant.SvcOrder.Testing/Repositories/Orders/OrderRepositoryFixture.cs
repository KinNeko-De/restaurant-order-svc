﻿using Dapper;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Repositories;
using Restaurant.SvcOrder.Repositories.Orders;

namespace Restaurant.SvcOrder.Testing.Repositories.Orders;

public class OrderRepositoryFixture
{
    public DatabaseConnectionProvider DatabaseConnectionProvider { get; }

    public OrderRepositoryFixture(DatabaseConnectionProvider databaseConnectionProvider)
    {
        DatabaseConnectionProvider = databaseConnectionProvider;
    }

    public static async Task SaveToLocalDatabase(Order order)
    {
        await SaveToDatabase(order);
    }

    public static async Task SaveToDatabase(Order order)
    {
        await order.Save(CancellationToken.None);
    }

    public static async Task<Order> LoadFromLocalDatabase(OrderId orderId)
    {
        var fixture = new OrderRepositoryFixture(new DatabaseFixture().GetConnectionProviderToLocalDatabase());
        return await fixture.LoadFromDatabase(orderId);
    }

    public async Task<Order> LoadFromDatabase(OrderId orderId)
    {
        var persistenceContext = CreatePersistenceContext();
        return await persistenceContext.Load(orderId, CancellationToken.None);
    }

    public async Task CleanupTables()
    {
        await using var connection = await DatabaseConnectionProvider.GetOpenConnection(CancellationToken.None);

        await CleanupTable(connection, "order_event");
        await CleanupTable(connection, "order_relation");
    }

    private Order.PersistenceContext CreatePersistenceContext()
    {
        return new Order.PersistenceContext(
            new OrderRepository(
            new NullLogger<OrderRepository>(),
            DatabaseConnectionProvider,
            new OrderSourceEventMapping()));
    }

    private static async Task CleanupTable(NpgsqlConnection connection, string table)
    {
        var commandDefinition = new CommandDefinition(
            $"delete from {table}",
            commandTimeout: TimeSpan.FromSeconds(5).Seconds,
            cancellationToken: CancellationToken.None);

        await connection.ExecuteAsync(commandDefinition);
    }
}
