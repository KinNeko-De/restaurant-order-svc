using Dapper;
using Google.Protobuf;
using Microsoft.Extensions.Localization;
using Npgsql;
using Restaurant.SvcOrder.Domain.Orders;
using Restaurant.SvcOrder.Domain.SourceEvents;

namespace Restaurant.SvcOrder.Repositories.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly ILogger<OrderRepository> logger;
    private readonly DatabaseConnectionProvider databaseConnectionProvider;
    private readonly OrderSourceEventMapping sourceEventMapping;

    private readonly int defaultCommandTimeout = TimeSpan.FromSeconds(5).Seconds;
    private const string RootName = "order";

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

    public async Task<IReadOnlyCollection<ISourceEvent>> GetSourceEventsByOrderId(Order order, OrderId orderId, CancellationToken cancellationToken)
    {
        await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

        var parameter = new
        {
            OrderId = orderId.Guid,
        };

        var commandDefinition = new CommandDefinition($@"SELECT
id AS {nameof(ReadSourceEvent.Id)},
sequence_number AS {nameof(ReadSourceEvent.SequenceNumber)},
type AS {nameof(ReadSourceEvent.Type)},
data AS {nameof(ReadSourceEvent.Data)}
FROM {RootName}_event 
where aggregate_root_id = @{nameof(parameter.OrderId)}",
            parameter,
            commandTimeout: defaultCommandTimeout,
            cancellationToken: cancellationToken);

        var sourceEvents = await connection.QueryAsync<ReadSourceEvent>(commandDefinition);

        IEnumerable<ISourceEvent> accountEvents = sourceEvents.OrderBy(x => x.SequenceNumber).Select(x => sourceEventMapping.MapMessageToSourceEvent(order, x));

        return accountEvents.ToArray();
    }

    public async Task SaveSourceEvents(OrderId orderId, IDictionary<int, ISourceEvent> addedSourceEvents, CancellationToken cancellationToken)
    {
        if (addedSourceEvents.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;

        await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

        try
        {
            await using var writer = await connection.BeginBinaryImportAsync("COPY order_event (id, aggregate_root_id, sequence_number, type, data, created_at) FROM STDIN (FORMAT BINARY)", cancellationToken);

            foreach (var addedSourceEvent in addedSourceEvents)
            {
                var sequenceNumber = addedSourceEvent.Key;
                ISourceEvent sourceEvent = addedSourceEvent.Value;
                IMessage datamodel = sourceEvent.ToDatamodel();

                await writer.StartRowAsync(cancellationToken);
                await writer.WriteAsync(sourceEvent.Id.Guid, cancellationToken);
                await writer.WriteAsync(orderId.Guid, cancellationToken);
                await writer.WriteAsync(sequenceNumber, cancellationToken);
                await writer.WriteAsync(datamodel.Descriptor.FullName, cancellationToken);
                await writer.WriteAsync(datamodel.ToByteArray(), cancellationToken);
                await writer.WriteAsync(now, cancellationToken);
            }

            await writer.CompleteAsync(cancellationToken);
        }

        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == $"ux_{RootName}_event__aggregate_root_id_sequence_number_key")
        {
            throw new OptimisticLockingException(orderId);
        }
    }

    /// <summary>
    /// Used for unique constraints for specific values and keys of aggregate
    /// Must exists for every entity inside a aggregate
    /// </summary>
    /// <param name="order"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SaveOrderRelation(Order order, CancellationToken cancellationToken)
    {
        await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

        var sequenceNumber = order.GetLastSourceEventSequenceNumber();
        var utcNow = DateTime.UtcNow;

        var parameter = new
        {
            Id = order.Id.Guid,
            SequenceNumber = sequenceNumber,
            RecordCreatedAt = utcNow
        };

        var commandDefinition = new CommandDefinition($@"INSERT INTO {RootName}_relation (
id,
sequence_number,
record_created_at
) VALUES (
@{nameof(parameter.Id)},
@{nameof(parameter.SequenceNumber)},
@{nameof(parameter.RecordCreatedAt)}
)",
            parameter,
            commandTimeout: defaultCommandTimeout,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);
    }

    public async Task UpdateOrderRelation(Order order, CancellationToken cancellationToken)
    {
        await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

        var sequenceNumber = order.GetLastSourceEventSequenceNumber();
        var utcNow = DateTime.UtcNow;

        var parameter = new
        {
            Id = order.Id.Guid,
            SequenceNumber = sequenceNumber,
            RecordUpdatedAt = utcNow
        };

        var commandDefinition = new CommandDefinition($@"UPDATE {RootName}_relation SET 
sequence_number = @{nameof(parameter.SequenceNumber)}, 
record_updated_at = @{nameof(parameter.RecordUpdatedAt)} 
WHERE {RootName}_relation.id =  @{nameof(parameter.Id)} AND {RootName}_relation.sequence_number < @{nameof(parameter.SequenceNumber)}",
            parameter,
            commandTimeout: defaultCommandTimeout,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(commandDefinition);
    }
}