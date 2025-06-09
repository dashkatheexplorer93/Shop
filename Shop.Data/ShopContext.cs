using Microsoft.EntityFrameworkCore;
using Shop.Data.Entities;

namespace Shop.Data;

public class ShopContext(DbContextOptions<ShopContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category configuration
    modelBuilder.Entity<Category>(entity =>
    {
        entity.HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Product configuration
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Customer configuration
    modelBuilder.Entity<Customer>(entity =>
    {
        entity.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Order configuration
    modelBuilder.Entity<Order>(entity =>
    {
        entity.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    });

    // OrderItem configuration
    modelBuilder.Entity<OrderItem>(entity =>
    {
        entity.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // Indexes
    modelBuilder.Entity<Category>()
        .HasIndex(c => c.Name);

    modelBuilder.Entity<Product>()
        .HasIndex(p => p.Name);

    modelBuilder.Entity<Customer>()
        .HasIndex(c => c.Email)
        .IsUnique();

    modelBuilder.Entity<Order>()
        .HasIndex(o => o.OrderDate);
    }
}
