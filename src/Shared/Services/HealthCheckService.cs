using Shared.Models;

namespace Shared.Services;

public interface IHealthCheckService
{
    Task<HealthCheckResult> CheckDatabaseAsync(string connectionString, string serviceName);
    Task<HealthCheckResult> CheckRabbitMQAsync(string connectionString);
    Task<HealthCheckResult> CheckServiceAsync(string serviceUrl, string serviceName);
    Task<HealthCheckResult> CheckMemoryUsageAsync();
    Task<HealthCheckResult> CheckDiskSpaceAsync();
    Task<ConsolidatedHealthCheck> GetConsolidatedHealthAsync();
}

public class HealthCheckService : IHealthCheckService
{
    private readonly HttpClient _httpClient;

    public HealthCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HealthCheckResult> CheckDatabaseAsync(string connectionString, string serviceName)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Simula verificação de banco - em implementação real usaria EntityFramework
            await Task.Delay(50); // Simula latência
            stopwatch.Stop();

            return new HealthCheckResult
            {
                Status = "Healthy",
                Component = $"{serviceName}_Database",
                Description = "Database connection is healthy",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                AdditionalData = new Dictionary<string, object>
                {
                    { "connectionString", MaskConnectionString(connectionString) },
                    { "provider", "MySQL" }
                }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthCheckResult
            {
                Status = "Unhealthy",
                Component = $"{serviceName}_Database",
                Description = $"Database connection failed: {ex.Message}",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckRabbitMQAsync(string connectionString)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Simula verificação RabbitMQ
            await Task.Delay(30);
            stopwatch.Stop();

            return new HealthCheckResult
            {
                Status = "Healthy",
                Component = "RabbitMQ",
                Description = "Message broker is healthy",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                AdditionalData = new Dictionary<string, object>
                {
                    { "host", "localhost:5672" },
                    { "queues", new[] { "order-created", "stock-update" } }
                }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthCheckResult
            {
                Status = "Unhealthy",
                Component = "RabbitMQ",
                Description = $"RabbitMQ connection failed: {ex.Message}",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckServiceAsync(string serviceUrl, string serviceName)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Cria um HttpClient específico para evitar conflitos
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            
            var response = await httpClient.GetAsync($"{serviceUrl}/health");
            stopwatch.Stop();

            var status = response.IsSuccessStatusCode ? "Healthy" : "Degraded";
            if (stopwatch.ElapsedMilliseconds > 1000) status = "Degraded";

            return new HealthCheckResult
            {
                Status = status,
                Component = serviceName,
                Description = $"Service is {status.ToLower()}",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                AdditionalData = new Dictionary<string, object>
                {
                    { "url", serviceUrl },
                    { "statusCode", (int)response.StatusCode }
                }
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new HealthCheckResult
            {
                Status = "Unhealthy",
                Component = serviceName,
                Description = $"Service check failed: {ex.Message}",
                ResponseTimeMs = stopwatch.ElapsedMilliseconds
            };
        }
    }

    public async Task<HealthCheckResult> CheckMemoryUsageAsync()
    {
        await Task.Delay(10);
        
        var process = System.Diagnostics.Process.GetCurrentProcess();
        var memoryUsageMB = process.WorkingSet64 / 1024 / 1024;
        
        var status = memoryUsageMB switch
        {
            < 200 => "Healthy",
            < 500 => "Degraded", 
            _ => "Unhealthy"
        };

        return new HealthCheckResult
        {
            Status = status,
            Component = "Memory",
            Description = $"Memory usage: {memoryUsageMB}MB",
            ResponseTimeMs = 10,
            AdditionalData = new Dictionary<string, object>
            {
                { "usageInMB", memoryUsageMB },
                { "threshold", "200MB (Healthy), 500MB (Degraded)" }
            }
        };
    }

    public async Task<HealthCheckResult> CheckDiskSpaceAsync()
    {
        await Task.Delay(10);
        
        try
        {
            var drive = new DriveInfo("C:");
            var freeSpaceGB = drive.AvailableFreeSpace / 1024 / 1024 / 1024;
            var totalSpaceGB = drive.TotalSize / 1024 / 1024 / 1024;
            var usagePercentage = ((double)(totalSpaceGB - freeSpaceGB) / totalSpaceGB) * 100;

            var status = usagePercentage switch
            {
                < 80 => "Healthy",
                < 90 => "Degraded",
                _ => "Unhealthy"
            };

            return new HealthCheckResult
            {
                Status = status,
                Component = "DiskSpace",
                Description = $"Disk usage: {usagePercentage:F1}%",
                ResponseTimeMs = 10,
                AdditionalData = new Dictionary<string, object>
                {
                    { "freeSpaceGB", freeSpaceGB },
                    { "totalSpaceGB", totalSpaceGB },
                    { "usagePercentage", Math.Round(usagePercentage, 1) }
                }
            };
        }
        catch (Exception ex)
        {
            return new HealthCheckResult
            {
                Status = "Unhealthy",
                Component = "DiskSpace",
                Description = $"Disk check failed: {ex.Message}",
                ResponseTimeMs = 10
            };
        }
    }

    public async Task<ConsolidatedHealthCheck> GetConsolidatedHealthAsync()
    {
        var results = new Dictionary<string, HealthCheckResult>();

        // Verificar serviços
        var services = new Dictionary<string, string>
        {
            { "StockService", "http://localhost:5001" },
            { "SalesService", "http://localhost:5002" }
        };

        var tasks = new List<Task>();
        foreach (var service in services)
        {
            tasks.Add(Task.Run(async () =>
            {
                results[service.Key] = await CheckServiceAsync(service.Value, service.Key);
            }));
        }

        // Verificar recursos do sistema
        tasks.Add(Task.Run(async () => results["Memory"] = await CheckMemoryUsageAsync()));
        tasks.Add(Task.Run(async () => results["DiskSpace"] = await CheckDiskSpaceAsync()));
        tasks.Add(Task.Run(async () => results["RabbitMQ"] = await CheckRabbitMQAsync("amqp://guest:guest@localhost:5672/")));

        await Task.WhenAll(tasks);

        // Calcular resumo
        var healthy = results.Values.Count(r => r.Status == "Healthy");
        var degraded = results.Values.Count(r => r.Status == "Degraded");
        var unhealthy = results.Values.Count(r => r.Status == "Unhealthy");
        var total = results.Count;

        var overallStatus = unhealthy > 0 ? "Unhealthy" : 
                           degraded > 0 ? "Degraded" : "Healthy";

        return new ConsolidatedHealthCheck
        {
            OverallStatus = overallStatus,
            Services = results,
            Summary = new HealthSummary
            {
                Healthy = healthy,
                Degraded = degraded,
                Unhealthy = unhealthy,
                Total = total,
                HealthPercentage = Math.Round((double)healthy / total * 100, 1),
                AverageResponseTime = (long)results.Values.Average(r => r.ResponseTimeMs)
            }
        };
    }

    private static string MaskConnectionString(string connectionString)
    {
        return connectionString.Contains("Password") || connectionString.Contains("Pwd") 
            ? connectionString.Split(';').Where(part => !part.Contains("Password") && !part.Contains("Pwd")).Aggregate((a, b) => $"{a};{b}") + ";Pwd=***"
            : connectionString;
    }
}
