using Microsoft.EntityFrameworkCore;
using SalesService.Data;
using Shared.Models;
using Shared.Services;
using Shared.Events;

namespace SalesService.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<Order?> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status);
}

public class OrderService : IOrderService
{
    protected readonly SalesDbContext _context;
    protected readonly IStockServiceClient _stockServiceClient;
    protected readonly IRabbitMQService _rabbitMQService;
    protected readonly ILogger<OrderService> _logger;

    public OrderService(
        SalesDbContext context,
        IStockServiceClient stockServiceClient,
        IRabbitMQService rabbitMQService,
        ILogger<OrderService> logger)
    {
        _context = context;
        _stockServiceClient = stockServiceClient;
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.CustomerId.ToString() == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public virtual async Task<Order?> CreateOrderAsync(CreateOrderRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var order = new Order
            {
                CustomerId = request.CustomerId,
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Items = new List<OrderItem>(),
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0;

            foreach (var item in request.Items)
            {
                // Verifica se o produto existe no StockService
                var product = await _stockServiceClient.GetProductAsync(item.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Produto {item.ProductId} não encontrado");
                }

                // estoque diretamente nos dados do produto
                if (product.StockQuantity < item.Quantity)
                {
                    throw new InvalidOperationException($"Produto {item.ProductId} não tem estoque suficiente. Disponível: {product.StockQuantity}, Solicitado: {item.Quantity}");
                }

                _logger.LogInformation("Produto {ProductId} tem estoque suficiente: {StockQuantity} >= {RequestedQuantity}", 
                    item.ProductId, product.StockQuantity, item.Quantity);

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.Name,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };

                order.Items.Add(orderItem);
                totalAmount += orderItem.TotalPrice;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Publica evento para atualizar estoque
            var stockUpdateEvents = order.Items.Select(item => new StockUpdateEvent
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Operation = "decrease",
                OrderId = order.Id.ToString()
            });

            foreach (var stockEvent in stockUpdateEvents)
            {
                _rabbitMQService.PublishMessage("stock-update", stockEvent);
            }

            // Publica evento de pedido criado
            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = request.CustomerId.ToString(),
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => new OrderItemEvent
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _rabbitMQService.PublishMessage("order-created", orderCreatedEvent);

            await transaction.CommitAsync();

            order.Status = OrderStatus.Processing;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pedido {OrderId} criado com sucesso para o usuário {UserId}", order.Id, request.CustomerId);

            return order;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Erro ao criar pedido para o usuário {CustomerId}", request.CustomerId);
            throw;
        }
    }

    public async Task<Order?> UpdateOrderStatusAsync(int id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return null;

        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return order;
    }
}

public class CreateOrderRequest
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}

public class CreateOrderItemRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}
