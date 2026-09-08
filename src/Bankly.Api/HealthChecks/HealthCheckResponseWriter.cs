using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bankly.Api.HealthChecks;

public static class HealthCheckResponseWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var isDevelopment = context.RequestServices
            .GetRequiredService<IWebHostEnvironment>()
            .IsDevelopment();

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                durationMs = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                error = isDevelopment ? e.Value.Exception?.Message : null
            })
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        return context.Response.WriteAsync(json);
    }
}