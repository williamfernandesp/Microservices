using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Fcg.Observability;

public static class HealthCheckExtensions
{
    public static WebApplication UseObservabilityEndpoints(this WebApplication app)
    {
        // Health check endpoint with detailed response
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = WriteHealthCheckResponse,
            AllowCachingResponses = false
        });

        // Liveness probe - simple check that the app is running
        app.MapGet("/health/live", () => Results.Ok(new { status = "Alive", timestamp = DateTime.UtcNow }))
            .AllowAnonymous()
            .WithTags("Health");

        // Readiness probe - checks dependencies
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthCheckResponse,
            AllowCachingResponses = false
        });

        return app;
    }

    private static async Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDuration = report.TotalDuration.TotalMilliseconds,
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                duration = entry.Value.Duration.TotalMilliseconds,
                description = entry.Value.Description,
                exception = entry.Value.Exception?.Message,
                data = entry.Value.Data
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        }));
    }
}
