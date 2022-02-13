using CodeFirstDemo.Application.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeFirstDemo.Application.Infrastructure
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions opt) : base(opt) { }
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
            modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
            
            // Optional. EF Core configures this relation by convention:
            modelBuilder.Entity<Store>().HasOne(s => s.Manager).WithOne(u => u.Store).HasForeignKey<Store>(s => s.ManagerId);
            modelBuilder.Entity<Store>().HasAlternateKey(s => s.Guid);
            modelBuilder.Entity<Store>().Property(s => s.Guid).ValueGeneratedOnAdd();
            // Convert naming convention from PropertyName to T_Property (useless, but good for demonstration)
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    foreach (var property in entity.GetProperties())
            //    {
            //        property.SetColumnName($"{entity.GetTableName()?.Substring(0, 1)}_{property.Name}");
            //    }
            //    entity.SetTableName(entity.GetTableName()?.ToUpper());
            //}
        }
    }
}
