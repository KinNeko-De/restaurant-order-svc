using System.Collections.Concurrent;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Restaurant.SvcOrder.Domain.Orders.SourceEvents;
using Restaurant.SvcOrder.Domain.SourceEvents;
using Restaurant.SvcOrder.Repositories.Orders.SourceEvents;

namespace Restaurant.SvcOrder.Repositories.Orders;

/// <summary>
/// Dependency injection singleton
/// </summary>
public class OrderSourceEventMapping
{
    private readonly ConcurrentDictionary<string, MessageDescriptor> mapperDescriptors = new();
    private readonly ConcurrentDictionary<string, Func<SourceEventId, IMessage, ISourceEvent>> mapperSourceEvents = new();

    public OrderSourceEventMapping()
    {
        mapperDescriptors.TryAdd(OrderCreatedV1.Descriptor.FullName, OrderCreatedV1.Descriptor);
        mapperSourceEvents.TryAdd(OrderCreatedV1.Descriptor.FullName, (sourceEventId, message) => OrderCreatedMapper.FromDatamodel(sourceEventId, (OrderCreatedV1)message));
        mapperSourceEvents.TryAdd(OrderCreatedV2.Descriptor.FullName, (sourceEventId, message) => OrderCreatedMapper.FromDatamodel(sourceEventId, (OrderCreatedV2)message));
    }

    public ISourceEvent MapMessageToSourceEvent(ReadOrderSourceEvent readOrderSourceEvent)
    {
        if (!mapperDescriptors.TryGetValue(readOrderSourceEvent.Type, out MessageDescriptor? messageDescriptor))
        {
            throw new InvalidOperationException($"Parsing source event {readOrderSourceEvent.Id} failed. Type {readOrderSourceEvent.Type} can not found in {GetType().FullName}.");
        }

        var protobufMessage = messageDescriptor.Parser.ParseFrom(readOrderSourceEvent.Data);
        
        if (!mapperSourceEvents.TryGetValue(readOrderSourceEvent.Type, out Func<SourceEventId, IMessage, ISourceEvent>? sourceEventFactory))
        {
            throw new InvalidOperationException($"Parsing source event {readOrderSourceEvent.Id} failed. Type {readOrderSourceEvent.Type} can not found in {GetType().FullName}.");
        }

        var sourceEventId = new SourceEventId(readOrderSourceEvent.Id);
        var sourceEvent = sourceEventFactory(sourceEventId, protobufMessage);
        return sourceEvent;
    }
}
