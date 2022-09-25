using Microsoft.Extensions.Options;
using Npgsql;
using Restaurant.SvcOrder.Repositories;

namespace Restaurant.SvcOrder.Testing.Repositories;
public class DatabaseFixture
{
    private DatabaseConnectionConfig ConnectToLocalDatabase()
    {
        return new DatabaseConnectionConfig()
        {
            Host = "localhost",
            Port = 23100,
            Database = "orderdatabase",
            User = "orderuser",
            Password = "orderpassword",
            MaxPoolSize = 5,
            SearchPath = "order",
            SslMode = SslMode.Prefer,
            TrustServerCertificate = true,
            Timeout = 30,
        };
    }

    /// <summary>
    /// For automated tests this connects you to your local running database
    /// </summary>
    /// <returns>DatabaseConnectionProvider to connect to local running test database</returns>
    public DatabaseConnectionProvider GetConnectionProviderToLocalDatabase()
    {
        IOptions<DatabaseConnectionConfig> databaseConnectionConfig = Options.Create(ConnectToLocalDatabase());
        var databaseConnectionProvider = new DatabaseConnectionProvider(databaseConnectionConfig);
        return databaseConnectionProvider;
    }
}
