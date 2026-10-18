using DeliveryManager.Model;
using Microsoft.EntityFrameworkCore;

namespace DeliveryManager.Infrastructure;

public class DeliveryContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DeliveryContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Product>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(r => r.Email).IsUnique();
        modelBuilder.Entity<OrderItem>().HasIndex("OrderId", "ProductId").IsUnique();

        modelBuilder.Entity<Restaurant>().OwnsOne(r => r.Address);
    }

    public void Seed()
    {
        // TODO: Add your seed code here.
    }
}
