using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace StockService.Data;

public class StockDbContext : DbContext
{
    public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.StockQuantity).HasColumnName("QuantityInStock").IsRequired();
        });

        // Dados iniciais
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Notebook Dell",
                Description = "Notebook Dell Inspiron 15",
                Price = 2500.00m,
                StockQuantity = 10
            },
            new Product
            {
                Id = 2,
                Name = "Mouse Logitech",
                Description = "Mouse sem fio Logitech MX Master",
                Price = 350.00m,
                StockQuantity = 25
            },
            new Product
            {
                Id = 3,
                Name = "Teclado Mecânico",
                Description = "Teclado mecânico RGB",
                Price = 450.00m,
                StockQuantity = 15
            }
        );
    }
}
