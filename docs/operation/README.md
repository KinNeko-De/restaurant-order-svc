# Logs
The application logs to the console.

# Metrics
Metrics are collected using the SystemDiagnostics.Metric package.
You can watch them using dotnet-counter

```cmd
dotnet-counters ps

dotnet-counters monitor -p <process_id> restaurant-order-svc

```

# HealthChecks
To see if a running application is still responding use the diagnostics or grpc health checks