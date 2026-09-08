using Bankly.Infrastructure.Persistence;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bankly.Api.HealthChecks;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddBanklyHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy("API no ar"), tags: ["self"])
            .AddDbContextCheck<BanklyContext>(
                name: "oracle-db",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready", "db"]);

        return services;
    }
}