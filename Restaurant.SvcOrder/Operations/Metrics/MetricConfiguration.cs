namespace Restaurant.SvcOrder.Operations.Metrics;

public class MetricConfiguration
{
    public required string Scheme { get; set; } = string.Empty;

    public required string Host { get; set; } = string.Empty;

    public int? Port { get; set; }

    public string? Path { get; set; }

    public string BuildUri()
    {
        return $"{Scheme}://{Host}{(Port != null ? $":{Port}": string.Empty)}{(Path != null ? $"/{Path}" : string.Empty)}";
    }
}
