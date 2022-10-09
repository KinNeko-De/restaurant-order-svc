using System.Diagnostics;
using Restaurant.SvcOrder.Repositories;

namespace Restaurant.SvcOrder.BackgroundServices;

/// <summary>
/// Checks if the configuration of the database is correct and a connection can be established.
/// After the background service ends.
/// </summary>
public class DatabaseConfigurationCheck : BackgroundService
{
    private readonly ILogger<DatabaseConfigurationCheck> logger;
    private readonly DatabaseConnectionProvider databaseConnectionProvider;

    public DatabaseConfigurationCheck(
        ILogger<DatabaseConfigurationCheck> logger,
        DatabaseConnectionProvider databaseConnectionProvider
        )
    {
        this.logger = logger;
        this.databaseConnectionProvider = databaseConnectionProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield(); // ensures that the background service is not blocking the start up of the service

        while (!cancellationToken.IsCancellationRequested)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                await using var connection = await databaseConnectionProvider.GetOpenConnection(cancellationToken);

                stopwatch.Stop();
                logger.LogInformation("DatabaseConnection could be established. Needed {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
                break;
            }
            catch (Exception exception)
            {
                stopwatch.Stop();
                logger.LogError(exception, "DatabaseConnection can not be established. Duration {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
            }
        }

        logger.LogInformation("Shutting downing after connection succeed.");
    }

}
