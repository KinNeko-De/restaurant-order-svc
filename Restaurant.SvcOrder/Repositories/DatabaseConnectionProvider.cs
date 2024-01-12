using Microsoft.Extensions.Options;
using Npgsql;

namespace Restaurant.SvcOrder.Repositories;

public class DatabaseConnectionProvider(
    IOptions<DatabaseConnectionConfiguration> connectionConfig
    )
{
    private readonly DatabaseConnectionConfiguration connectionConfig = connectionConfig.Value;
    private string? connectionString;

    public async Task<NpgsqlConnection> GetOpenConnection(CancellationToken cancellationToken)
    {
        EnsureConnectionStringIsCreated();

        var connection = new NpgsqlConnection(connectionString);

        await connection.OpenAsync(cancellationToken);

        return connection;
    }

    private void EnsureConnectionStringIsCreated()
    {
        if (connectionString == null)
        {
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = connectionConfig.Host,
                Port = connectionConfig.Port,
                Database = connectionConfig.Database,
                Username = connectionConfig.User,
                SearchPath = connectionConfig.SearchPath,
                Password = connectionConfig.Password,
                SslMode = connectionConfig.SslMode,
                MaxPoolSize = connectionConfig.MaxPoolSize,
                Timeout =connectionConfig.Timeout,
            };

            connectionString = builder.ConnectionString;
        }
    }
}
