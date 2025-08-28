using Shared.Services;
using Shared.Events;
using StockService.Services;

namespace StockService.Services;

public class StockUpdateListener : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockUpdateListener> _logger;

    public StockUpdateListener(
        IRabbitMQService rabbitMQService,
        IServiceProvider serviceProvider,
        ILogger<StockUpdateListener> logger)
    {
        _rabbitMQService = rabbitMQService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<StockUpdateEvent>("stock-update", HandleStockUpdate);
        return Task.CompletedTask;
    }

    private async void HandleStockUpdate(StockUpdateEvent stockUpdate)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

            var quantity = stockUpdate.Operation == "decrease" ? -stockUpdate.Quantity : stockUpdate.Quantity;
            
            var success = await productService.UpdateStockAsync(stockUpdate.ProductId, quantity);
            
            if (success)
            {
                _logger.LogInformation("Estoque atualizado: Produto {ProductId}, Quantidade {Quantity}, Operação {Operation}",
                    stockUpdate.ProductId, stockUpdate.Quantity, stockUpdate.Operation);
            }
            else
            {
                _logger.LogWarning("Falha ao atualizar estoque: Produto {ProductId}, Quantidade {Quantity}",
                    stockUpdate.ProductId, stockUpdate.Quantity);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar atualização de estoque");
        }
    }
}
