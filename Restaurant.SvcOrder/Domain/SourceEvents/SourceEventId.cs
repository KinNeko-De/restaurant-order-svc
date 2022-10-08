namespace Restaurant.SvcOrder.Domain.SourceEvents;

public readonly struct SourceEventId : IEquatable<SourceEventId>
{
    public static SourceEventId Empty { get; } = new(Guid.Empty);

    public Guid Guid { get; init; }

    public SourceEventId(Guid id)
    {
        Guid = id;
    }

    public static SourceEventId NewSourceEventId()
    {
        return new SourceEventId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }

    public static bool operator ==(SourceEventId id, SourceEventId other)
    {
        return id.Equals(other);
    }

    public static bool operator !=(SourceEventId id, SourceEventId other)
    {
        return !id.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is SourceEventId other && Equals(other);
    }

    public bool Equals(SourceEventId other)
    {
        return Guid.Equals(other.Guid);
    }
}
