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
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
            modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
        }
    }
}
