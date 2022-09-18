using Dapper;
using Npgsql;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Repositories.Orders;

public class OrderRepository
{
    private readonly ILogger<OrderRepository> logger;
    private readonly DatabaseConnectionProvider databaseConnectionProvider;
    private readonly OrderSourceEventMapping sourceEventMapping;

    private readonly int defaultCommandTimeout = TimeSpan.FromSeconds(5).Seconds;

    public OrderRepository(
        ILogger<OrderRepository> logger,
        DatabaseConnectionProvider databaseConnectionProvider,
        OrderSourceEventMapping sourceEventMapping
    )
    {
        this.logger = logger;
        this.databaseConnectionProvider = databaseConnectionProvider;
        this.sourceEventMapping = sourceEventMapping;
    }

    public async Task<List<ISourceEvent>> GetSourceEventsByOperationalFileId(OrderId orderId, CancellationToken cancellationToken)
    {
        await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

        var parameter = new
        {
            OrderId = orderId.Guid,
        };

        var commandDefinition = new CommandDefinition($@"SELECT
id AS {nameof(ReadOrderSourceEvent.Id)},
sequence_number AS {nameof(ReadOrderSourceEvent.SequenceNumber)},
type AS {nameof(ReadOrderSourceEvent.Type)},
data AS {nameof(ReadOrderSourceEvent.Data)},
FROM order_event 
where aggregate_root_id = @{nameof(parameter.OrderId)}",
            parameter,
            commandTimeout: defaultCommandTimeout,
            cancellationToken: cancellationToken);

        var sourceEvents = await connection.QueryAsync<ReadOrderSourceEvent>(commandDefinition);

        IEnumerable<ISourceEvent> accountEvents = sourceEvents.OrderBy(x => x.SequenceNumber).Select(x => sourceEventMapping.MapMessageToSourceEvent(x));

        return accountEvents.ToList();
    }
}
