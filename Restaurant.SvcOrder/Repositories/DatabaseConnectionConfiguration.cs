using Npgsql;

namespace Restaurant.SvcOrder.Repositories;

public class DatabaseConnectionConfiguration
{
    public string? Host { get; set; }

    public int Port { get; set; }

    public string? Database { get; set; }

    public string? SearchPath { get; set; }

    public string? User { get; set; }

    public string? Password { get; set; }

    public SslMode SslMode { get; set; }

    public int MaxPoolSize { get; set; }

    public int Timeout { get; set; }
}
