using Shared.Services;
using Shared.Events;
using StockService.Services;

namespace StockService.Services;

public class StockUpdateBackgroundService : BackgroundService
{
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockUpdateBackgroundService> _logger;

    public StockUpdateBackgroundService(
        IRabbitMQService rabbitMQService,
        IServiceProvider serviceProvider,
        ILogger<StockUpdateBackgroundService> logger)
    {
        _rabbitMQService = rabbitMQService;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.Subscribe<StockUpdateEvent>("stock-update", async (stockUpdateEvent) =>
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService>();

                var quantity = stockUpdateEvent.Operation == "decrease" 
                    ? -stockUpdateEvent.Quantity 
                    : stockUpdateEvent.Quantity;

                var success = await productService.UpdateStockAsync(stockUpdateEvent.ProductId, quantity);

                if (success)
                {
                    _logger.LogInformation(
                        "Estoque atualizado: Produto {ProductId}, Quantidade {Quantity}, Operação {Operation}, Pedido {OrderId}",
                        stockUpdateEvent.ProductId,
                        stockUpdateEvent.Quantity,
                        stockUpdateEvent.Operation,
                        stockUpdateEvent.OrderId);
                }
                else
                {
                    _logger.LogWarning(
                        "Falha ao atualizar estoque: Produto {ProductId}, Quantidade {Quantity}, Operação {Operation}",
                        stockUpdateEvent.ProductId,
                        stockUpdateEvent.Quantity,
                        stockUpdateEvent.Operation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar atualização de estoque");
            }
        });

        return Task.CompletedTask;
    }
}
