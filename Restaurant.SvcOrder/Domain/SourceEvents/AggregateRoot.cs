using System.Transactions;
using Restaurant.SvcOrder.Domain.Orders;

namespace Restaurant.SvcOrder.Domain.SourceEvents;

public class AggregateRoot: IAggregateRoot
{
    protected readonly List<ISourceEvent> AppliedSourceEvents = new();
    protected readonly Dictionary<int, ISourceEvent> NotPersistedSourceEvents = new();

    /// <summary>
    /// Only call from types of the aggregate
    /// </summary>
    /// <param name="sourceEvent"></param>
    protected void NewSourceEvent(ISourceEvent sourceEvent)
    {
        AppliedSourceEvents.Add(sourceEvent);
        NotPersistedSourceEvents.Add(AppliedSourceEvents.Count, sourceEvent);
    }

    protected bool NeedsToBeSaved()
    {
        return NotPersistedSourceEvents.Any();
    }

    protected bool IsNew()
    {
        return AppliedSourceEvents.Count == NotPersistedSourceEvents.Count;
    }

    protected void Saved()
    {
        NotPersistedSourceEvents.Clear();
    }

    public int GetLastSourceEventSequenceNumber()
    {
        return AppliedSourceEvents.Count;
    }
}
