using System.Collections.Concurrent;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders;

namespace Restaurant.SvcOrder.Repositories;

/// <summary>
/// Dependency injection singleton
/// </summary>
public class SourceEventMapping<TAggregateRoot> where  TAggregateRoot : IAggregateRoot
{
    protected readonly ConcurrentDictionary<string, (MessageDescriptor messageDescriptor, Func<TAggregateRoot, SourceEventId, IMessage, ISourceEvent> sourceEventFactory)> Mappers = new();

    public ISourceEvent MapMessageToSourceEvent(TAggregateRoot aggregateRoot, ReadSourceEvent readSourceEvent)
    {
        if (!Mappers.TryGetValue(readSourceEvent.Type, out (MessageDescriptor messageDescriptor, Func<TAggregateRoot, SourceEventId, IMessage, ISourceEvent> sourceEventFactory) mapper))
        {
            throw new InvalidOperationException($"Parsing source event {readSourceEvent.Id} failed. Type {readSourceEvent.Type} can not found in {GetType().FullName}.");
        }

        var protobufMessage = mapper.messageDescriptor.Parser.ParseFrom(readSourceEvent.Data);
        var sourceEventId = new SourceEventId(readSourceEvent.Id);
        var sourceEvent = mapper.sourceEventFactory(aggregateRoot, sourceEventId, protobufMessage);
        return sourceEvent;
    }
}
