namespace Restaurant.SvcOrder.Domain.Orders;

public readonly struct OrderId : IEquatable<OrderId>
{
    public static OrderId Empty { get; } = new(Guid.Empty);

    public Guid Guid { get; init; }

    public OrderId(Guid id)
    {
        Guid = id;
    }

    public static OrderId NewOrderId()
    {
        return new OrderId(Guid.NewGuid());
    }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public override int GetHashCode()
    {
        return Guid.GetHashCode();
    }

    public static bool operator ==(OrderId id, OrderId other)
    {
        return id.Equals(other);
    }

    public static bool operator !=(OrderId id, OrderId other)
    {
        return !id.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        return obj is OrderId other && Equals(other);
    }

    public bool Equals(OrderId other)
    {
        return Guid.Equals(other.Guid);
    }
}
