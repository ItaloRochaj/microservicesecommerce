using Microsoft.AspNetCore.Mvc;
using Shared.Services;
using System.Text.Json;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthCheckService _healthCheckService;
    private readonly ILogger<HealthController> _logger;

    public HealthController(IHealthCheckService healthCheckService, ILogger<HealthController> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    [HttpGet("consolidated")]
    public async Task<ActionResult> GetConsolidatedHealth()
    {
        try
        {
            var result = await _healthCheckService.GetConsolidatedHealthAsync();
            
            var statusCode = result.OverallStatus switch
            {
                "Healthy" => 200,
                "Degraded" => 200, // Still operational
                "Unhealthy" => 503, // Service Unavailable
                _ => 200
            };

            return StatusCode(statusCode, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar health consolidado");
            return StatusCode(500, new { error = "Erro interno ao verificar saúde do sistema" });
        }
    }

    [HttpGet("dashboard")]
    public async Task<ContentResult> GetHealthDashboard()
    {
        try
        {
            var healthData = await _healthCheckService.GetConsolidatedHealthAsync();
            var html = GenerateHealthDashboardHtml(healthData);
            
            return new ContentResult
            {
                Content = html,
                ContentType = "text/html",
                StatusCode = 200
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar dashboard de health");
            return new ContentResult
            {
                Content = "<h1>Erro ao carregar dashboard</h1>",
                ContentType = "text/html",
                StatusCode = 500
            };
        }
    }

    private string GenerateHealthDashboardHtml(Shared.Models.ConsolidatedHealthCheck healthData)
    {
        var statusIcon = healthData.OverallStatus switch
        {
            "Healthy" => "🟢",
            "Degraded" => "🟡", 
            "Unhealthy" => "🔴",
            _ => "⚪"
        };

        var statusColor = healthData.OverallStatus switch
        {
            "Healthy" => "#4CAF50",
            "Degraded" => "#FF9800",
            "Unhealthy" => "#F44336",
            _ => "#9E9E9E"
        };

        var servicesHtml = string.Join("", healthData.Services.Select(service => 
        {
            var serviceIcon = service.Value.Status switch
            {
                "Healthy" => "🟢",
                "Degraded" => "🟡",
                "Unhealthy" => "🔴",
                _ => "⚪"
            };

            var serviceColor = service.Value.Status switch
            {
                "Healthy" => "#4CAF50",
                "Degraded" => "#FF9800", 
                "Unhealthy" => "#F44336",
                _ => "#9E9E9E"
            };

            return $@"
                <div class='service-card' style='border-left: 4px solid {serviceColor};'>
                    <div class='service-header'>
                        <span class='service-icon'>{serviceIcon}</span>
                        <span class='service-name'>{service.Key}</span>
                        <span class='service-status' style='color: {serviceColor};'>{service.Value.Status}</span>
                    </div>
                    <div class='service-details'>
                        <div class='detail-item'>
                            <span class='detail-label'>Response Time:</span>
                            <span class='detail-value'>{service.Value.ResponseTimeMs}ms</span>
                        </div>
                        <div class='detail-item'>
                            <span class='detail-label'>Description:</span>
                            <span class='detail-value'>{service.Value.Description}</span>
                        </div>
                        <div class='detail-item'>
                            <span class='detail-label'>Last Checked:</span>
                            <span class='detail-value'>{service.Value.LastChecked:HH:mm:ss}</span>
                        </div>
                    </div>
                </div>";
        }));

        return $@"
<!DOCTYPE html>
<html lang='pt-BR'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>🏥 Microservices Health Dashboard</title>
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }}
        
        .container {{
            max-width: 1200px;
            margin: 0 auto;
            background: white;
            border-radius: 20px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            overflow: hidden;
        }}
        
        .header {{
            background: linear-gradient(135deg, {statusColor} 0%, {statusColor}CC 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        
        .header h1 {{
            font-size: 2.5em;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 15px;
        }}
        
        .header .subtitle {{
            font-size: 1.2em;
            opacity: 0.9;
        }}
        
        .summary-cards {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
            padding: 30px;
            background: #f8f9fa;
        }}
        
        .summary-card {{
            background: white;
            border-radius: 15px;
            padding: 25px;
            text-align: center;
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
            transition: transform 0.3s ease;
        }}
        
        .summary-card:hover {{
            transform: translateY(-5px);
        }}
        
        .summary-card .number {{
            font-size: 3em;
            font-weight: bold;
            color: {statusColor};
            margin-bottom: 10px;
        }}
        
        .summary-card .label {{
            font-size: 1.1em;
            color: #666;
            font-weight: 500;
        }}
        
        .services-container {{
            padding: 30px;
        }}
        
        .services-title {{
            font-size: 1.8em;
            margin-bottom: 25px;
            color: #333;
            display: flex;
            align-items: center;
            gap: 10px;
        }}
        
        .services-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
            gap: 20px;
        }}
        
        .service-card {{
            background: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
            transition: transform 0.3s ease, box-shadow 0.3s ease;
        }}
        
        .service-card:hover {{
            transform: translateY(-3px);
            box-shadow: 0 10px 25px rgba(0,0,0,0.15);
        }}
        
        .service-header {{
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #f0f0f0;
        }}
        
        .service-icon {{
            font-size: 1.5em;
        }}
        
        .service-name {{
            font-size: 1.3em;
            font-weight: bold;
            color: #333;
            flex-grow: 1;
        }}
        
        .service-status {{
            font-weight: bold;
            font-size: 1.1em;
        }}
        
        .service-details {{
            display: flex;
            flex-direction: column;
            gap: 12px;
        }}
        
        .detail-item {{
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}
        
        .detail-label {{
            color: #666;
            font-weight: 500;
        }}
        
        .detail-value {{
            color: #333;
            font-weight: bold;
        }}
        
        .footer {{
            background: #f8f9fa;
            padding: 20px;
            text-align: center;
            color: #666;
            border-top: 1px solid #eee;
        }}
        
        .refresh-btn {{
            background: {statusColor};
            color: white;
            border: none;
            padding: 12px 25px;
            border-radius: 25px;
            font-weight: bold;
            cursor: pointer;
            margin-top: 15px;
            transition: background 0.3s ease;
        }}
        
        .refresh-btn:hover {{
            background: {statusColor}DD;
        }}
        
        @keyframes pulse {{
            0% {{ opacity: 1; }}
            50% {{ opacity: 0.7; }}
            100% {{ opacity: 1; }}
        }}
        
        .auto-refresh {{
            animation: pulse 2s infinite;
        }}
    </style>
    <script>
        function refreshPage() {{
            window.location.reload();
        }}
        
        // Auto refresh every 30 seconds
        setInterval(refreshPage, 30000);
    </script>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>
                {statusIcon}
                <span>Microservices Health Dashboard</span>
            </h1>
            <div class='subtitle'>Status Geral: {healthData.OverallStatus} | Última Atualização: {healthData.Timestamp:HH:mm:ss}</div>
        </div>
        
        <div class='summary-cards'>
            <div class='summary-card'>
                <div class='number'>{healthData.Summary.Healthy}</div>
                <div class='label'>🟢 Healthy</div>
            </div>
            <div class='summary-card'>
                <div class='number'>{healthData.Summary.Degraded}</div>
                <div class='label'>🟡 Degraded</div>
            </div>
            <div class='summary-card'>
                <div class='number'>{healthData.Summary.Unhealthy}</div>
                <div class='label'>🔴 Unhealthy</div>
            </div>
            <div class='summary-card'>
                <div class='number'>{healthData.Summary.HealthPercentage}%</div>
                <div class='label'>📊 Health Score</div>
            </div>
            <div class='summary-card'>
                <div class='number'>{healthData.Summary.AverageResponseTime}ms</div>
                <div class='label'>⚡ Avg Response</div>
            </div>
        </div>
        
        <div class='services-container'>
            <h2 class='services-title'>
                🔧 Services Status
            </h2>
            <div class='services-grid'>
                {servicesHtml}
            </div>
        </div>
        
        <div class='footer'>
            <div>🚀 Microservices E-commerce | Ambiente: {healthData.Environment} | Versão: {healthData.Version}</div>
            <button class='refresh-btn auto-refresh' onclick='refreshPage()'>🔄 Atualizar Agora</button>
            <div style='margin-top: 10px; font-size: 0.9em;'>
                ⏱️ Atualização automática a cada 30 segundos
            </div>
        </div>
    </div>
</body>
</html>";
    }
}
