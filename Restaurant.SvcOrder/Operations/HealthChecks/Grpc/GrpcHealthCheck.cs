using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Restaurant.SvcOrder.Operations.HealthChecks.Grpc;

public class GrpcHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
