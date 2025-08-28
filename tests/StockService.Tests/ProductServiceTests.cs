using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using StockService.Data;
using StockService.Services;
using Shared.Models;
using Xunit;

namespace StockService.Tests;

public class ProductServiceTests : IDisposable
{
    private readonly StockDbContext _context;
    private readonly ProductService _productService;
    private readonly Mock<ILogger<ProductService>> _mockLogger;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<StockDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new StockDbContext(options);
        _mockLogger = new Mock<ILogger<ProductService>>();
        _productService = new ProductService(_context);

        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Produto Teste 1",
                Description = "Descrição do produto teste 1",
                Price = 100.00m,
                StockQuantity = 10
            },
            new Product
            {
                Id = 2,
                Name = "Produto Teste 2",
                Description = "Descrição do produto teste 2",
                Price = 200.00m,
                StockQuantity = 5
            }
        };

        _context.Products.AddRange(products);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Produto Teste 1", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var productId = 999;

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidProduct_ShouldCreateProduct()
    {
        // Arrange
        var newProduct = new Product
        {
            Name = "Novo Produto",
            Description = "Descrição do novo produto",
            Price = 150.00m,
            StockQuantity = 20
        };

        // Act
        var result = await _productService.CreateProductAsync(newProduct);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Novo Produto", result.Name);
        
        var productInDb = await _context.Products.FindAsync(result.Id);
        Assert.NotNull(productInDb);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidProduct_ShouldUpdateProduct()
    {
        // Arrange
        var productId = 1;
        var updatedProduct = new Product
        {
            Name = "Produto Atualizado",
            Description = "Descrição atualizada",
            Price = 120.00m,
            StockQuantity = 15
        };

        // Act
        var result = await _productService.UpdateProductAsync(productId, updatedProduct);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Produto Atualizado", result.Name);
        Assert.Equal(120.00m, result.Price);
        Assert.Equal(15, result.StockQuantity);
    }

    [Fact]
    public async Task UpdateStockAsync_WithValidQuantity_ShouldUpdateStock()
    {
        // Arrange
        var productId = 1;
        var quantityToAdd = 5;

        // Act
        var result = await _productService.UpdateStockAsync(productId, quantityToAdd);

        // Assert
        Assert.True(result);
        
        var product = await _context.Products.FindAsync(productId);
        Assert.Equal(15, product!.StockQuantity); // 10 + 5
    }

    [Fact]
    public async Task UpdateStockAsync_WithInsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var productId = 1;
        var quantityToRemove = -15; // Tentando remover mais do que tem

        // Act
        var result = await _productService.UpdateStockAsync(productId, quantityToRemove);

        // Assert
        Assert.False(result);
        
        var product = await _context.Products.FindAsync(productId);
        Assert.Equal(10, product!.StockQuantity); // Deve permanecer 10
    }

    [Fact]
    public async Task CheckStockAvailabilityAsync_WithSufficientStock_ShouldReturnTrue()
    {
        // Arrange
        var productId = 1;
        var quantityNeeded = 5;

        // Act
        var result = await _productService.CheckStockAvailabilityAsync(productId, quantityNeeded);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckStockAvailabilityAsync_WithInsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var productId = 1;
        var quantityNeeded = 15; // Mais do que tem em estoque

        // Act
        var result = await _productService.CheckStockAvailabilityAsync(productId, quantityNeeded);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _productService.DeleteProductAsync(productId);

        // Assert
        Assert.True(result);
        
        var product = await _context.Products.FindAsync(productId);
        Assert.Null(product);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
