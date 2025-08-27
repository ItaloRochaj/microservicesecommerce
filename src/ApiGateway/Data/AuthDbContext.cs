using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace ApiGateway.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.FullName).HasMaxLength(100);

            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Usuário padrão para testes
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123");
        var userPasswordHash = BCrypt.Net.BCrypt.HashPassword("user123");

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@test.com",
                PasswordHash = adminPasswordHash,
                FullName = "Administrador",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = 2,
                Username = "user",
                Email = "user@test.com",
                PasswordHash = userPasswordHash,
                FullName = "Usuário Teste",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}
