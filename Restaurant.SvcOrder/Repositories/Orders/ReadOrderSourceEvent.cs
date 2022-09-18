namespace Restaurant.SvcOrder.Repositories.Orders;

public class ReadOrderSourceEvent
{
    public Guid Id { get; init; }

    public string Type { get; init; } = string.Empty;

    public byte[] Data { get; init; } = Array.Empty<byte>();

    public int SequenceNumber { get; init; }
}
