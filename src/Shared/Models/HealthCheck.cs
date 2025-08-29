using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class HealthCheckResult
{
    public string Status { get; set; } = string.Empty;
    public string Component { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long ResponseTimeMs { get; set; }
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class ConsolidatedHealthCheck
{
    public string OverallStatus { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, HealthCheckResult> Services { get; set; } = new();
    public HealthSummary Summary { get; set; } = new();
    public string Environment { get; set; } = "Development";
    public string Version { get; set; } = "1.0.0";
}

public class HealthSummary
{
    public int Healthy { get; set; }
    public int Degraded { get; set; }
    public int Unhealthy { get; set; }
    public int Total { get; set; }
    public double HealthPercentage { get; set; }
    public long AverageResponseTime { get; set; }
}
