using Google.Protobuf;

namespace Restaurant.SvcOrder.Domain.SourceEvents;

public interface ISourceEvent
{
    SourceEventId Id { get; init; }
}
