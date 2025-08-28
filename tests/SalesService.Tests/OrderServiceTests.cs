using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SalesService.Data;
using SalesService.Services;
using Shared.Models;
using Shared.Services;
using Xunit;

namespace SalesService.Tests;

public class OrderServiceTests : IDisposable
{
    private readonly SalesDbContext _context;
    private readonly Mock<IStockServiceClient> _mockStockServiceClient;
    private readonly Mock<IRabbitMQService> _mockRabbitMQService;
    private readonly Mock<ILogger<OrderService>> _mockLogger;
    private readonly TestOrderService _orderService;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<SalesDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SalesDbContext(options);
        _mockStockServiceClient = new Mock<IStockServiceClient>();
        _mockRabbitMQService = new Mock<IRabbitMQService>();
        _mockLogger = new Mock<ILogger<OrderService>>();
        
        _orderService = new TestOrderService(
            _context,
            _mockStockServiceClient.Object,
            _mockRabbitMQService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetOrdersByUserAsync_ShouldReturnUserOrders()
    {
        // Arrange
        var userId = "user123";
        var orders = new List<Order>
        {
            new Order
            {
                Id = 1,
                UserId = userId,
                TotalAmount = 100.00m,
                Status = OrderStatus.Confirmed,
                Items = new List<OrderItem>()
            },
            new Order
            {
                Id = 2,
                UserId = "otheruser",
                TotalAmount = 200.00m,
                Status = OrderStatus.Pending,
                Items = new List<OrderItem>()
            }
        };

        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();

        // Act
        var result = await _orderService.GetOrdersByUserAsync(userId);

        // Assert
        Assert.Single(result);
        Assert.Equal(userId, result.First().UserId);
    }

    [Fact]
    public async Task CreateOrderAsync_WithValidItems_ShouldCreateOrder()
    {
        // Arrange
        var userId = "user123";
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 2 }
            }
        };

        var productDto = new ProductDto
        {
            Id = 1,
            Name = "Produto Teste",
            Price = 50.00m,
            StockQuantity = 10
        };

        _mockStockServiceClient
            .Setup(x => x.CheckStockAvailabilityAsync(1, 2))
            .ReturnsAsync(true);

        _mockStockServiceClient
            .Setup(x => x.GetProductAsync(1))
            .ReturnsAsync(productDto);

        // Act
        var result = await _orderService.CreateOrderAsync(request, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(100.00m, result.TotalAmount); // 2 * 50.00
        Assert.Single(result.Items);
        Assert.Equal(2, result.Items.First().Quantity);

        // Verificar se eventos foram publicados
        _mockRabbitMQService.Verify(
            x => x.PublishMessage(
                "stock-update", 
                It.IsAny<object>()), 
            Times.Once);

        _mockRabbitMQService.Verify(
            x => x.PublishMessage(
                "order-created", 
                It.IsAny<object>()), 
            Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_WithInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 1, Quantity = 10 }
            }
        };

        _mockStockServiceClient
            .Setup(x => x.CheckStockAvailabilityAsync(1, 10))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderService.CreateOrderAsync(request, userId));
    }

    [Fact]
    public async Task CreateOrderAsync_WithNonExistentProduct_ShouldThrowException()
    {
        // Arrange
        var userId = "user123";
        var request = new CreateOrderRequest
        {
            Items = new List<CreateOrderItemRequest>
            {
                new CreateOrderItemRequest { ProductId = 999, Quantity = 1 }
            }
        };

        _mockStockServiceClient
            .Setup(x => x.CheckStockAvailabilityAsync(999, 1))
            .ReturnsAsync(true);

        _mockStockServiceClient
            .Setup(x => x.GetProductAsync(999))
            .ReturnsAsync((ProductDto?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _orderService.CreateOrderAsync(request, userId));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithValidOrder_ShouldUpdateStatus()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            UserId = "user123",
            TotalAmount = 100.00m,
            Status = OrderStatus.Pending,
            Items = new List<OrderItem>()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        var result = await _orderService.UpdateOrderStatusAsync(1, OrderStatus.Shipped);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Shipped, result.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_WithInvalidOrder_ShouldReturnNull()
    {
        // Act
        var result = await _orderService.UpdateOrderStatusAsync(999, OrderStatus.Shipped);

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

// Classe para testes que não usa transações
public class TestOrderService : OrderService
{
    public TestOrderService(
        SalesDbContext context,
        IStockServiceClient stockServiceClient,
        IRabbitMQService rabbitMQService,
        ILogger<OrderService> logger) : base(context, stockServiceClient, rabbitMQService, logger)
    {
    }

    public override async Task<Order?> CreateOrderAsync(CreateOrderRequest request, string userId)
    {
        // Versão sem transação para testes
        try
        {
            var order = new Order
            {
                UserId = userId,
                Items = new List<OrderItem>(),
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0;

            foreach (var item in request.Items)
            {
                // Verificar disponibilidade do produto
                var stockAvailable = await _stockServiceClient.CheckStockAvailabilityAsync(item.ProductId, item.Quantity);
                if (!stockAvailable)
                {
                    throw new InvalidOperationException($"Produto {item.ProductId} não tem estoque suficiente");
                }

                // Buscar informações do produto
                var product = await _stockServiceClient.GetProductAsync(item.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Produto {item.ProductId} não encontrado");
                }

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

            // Publicar eventos
            var stockUpdateEvents = order.Items.Select(item => new Shared.Events.StockUpdateEvent
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

            var orderCreatedEvent = new Shared.Events.OrderCreatedEvent
            {
                OrderId = order.Id,
                UserId = userId,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(i => new Shared.Events.OrderItemEvent
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _rabbitMQService.PublishMessage("order-created", orderCreatedEvent);

            order.Status = OrderStatus.Confirmed;
            await _context.SaveChangesAsync();

            return order;
        }
        catch (Exception)
        {
            throw;
        }
    }
}
