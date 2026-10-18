using RichDomainModelDemo.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RichDomainModelDemo.Application.Infrastructure
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions opt) : base(opt) { }
        public DbSet<ConfirmedOrder> ConfirmedOrders => Set<ConfirmedOrder>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Store> Stores => Set<Store>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
            modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
            modelBuilder.Entity<Customer>().OwnsOne(c => c.Address);
            modelBuilder.Entity<Order>().OwnsOne(o => o.ShippingAddress);
            modelBuilder.Entity<Order>().HasDiscriminator(o => o.OrderType);
        }
    }
}
