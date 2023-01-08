using System.Diagnostics.Metrics;

namespace Restaurant.SvcOrder.Operations.Metrics;

public class Metric
{
    public const string ApplicationName = "restaurant-order-svc";

    private static readonly Meter Meter = new Meter(ApplicationName, "0.0.1");

    private static readonly Counter<int> DatabaseConnectionError = Meter.CreateCounter<int>("database-connection-error", "error", "Connection errors to the database");
    private static readonly Counter<int> TestRequest = Meter.CreateCounter<int>("request-test", "request", "just for testing");


    public void DatabaseConnectionErrorOccurred()
    {
        DatabaseConnectionError.Add(1);
    }

    public void TestRequestStarted()
    {
        TestRequest.Add(1);
    }

}
