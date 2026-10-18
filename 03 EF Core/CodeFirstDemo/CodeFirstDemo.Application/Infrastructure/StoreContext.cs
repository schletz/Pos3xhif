using Bogus;
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
            modelBuilder.Entity<Offer>().HasIndex("StoreId", "ProductEan").IsUnique();

            // Optional. EF Core configures this relation by convention:
            modelBuilder.Entity<Store>().HasOne(s => s.Manager)
                .WithOne(u => u.Store)
                .HasForeignKey<Store>("ManagerId")
                .OnDelete(DeleteBehavior.Cascade); // Delete Store if Manager is deleted. Important to change Manager.
            modelBuilder.Entity<Store>().HasAlternateKey(s => s.Guid);
            modelBuilder.Entity<Store>().Property(s => s.Guid).ValueGeneratedOnAdd();
        }
        public void Seed()
        {
            Randomizer.Seed = new Random(1335);
            var productCategories = new Faker<ProductCategory>("de")
                .CustomInstantiator(f => new ProductCategory(name: f.Commerce.ProductAdjective())
                {
                    NameEn = f.Commerce.ProductAdjective().OrNull(f, 0.5f)
                })
                .Generate(5)
                .ToList();
            ProductCategories.AddRange(productCategories);
            SaveChanges();

            var products = new Faker<Product>("de")
                .CustomInstantiator(f => new Product(
                    ean: f.Random.Int(1000, 9999),
                    name: f.Commerce.ProductName(),
                    productCategory: f.Random.ListItem(productCategories)))
                .Generate(25)
                .DistinctBy(p => p.Ean)
                .ToList();
            Products.AddRange(products);
            SaveChanges();

            var stores = new Faker<Store>("de")
                .CustomInstantiator(f => new Store(
                    name: f.Company.CompanyName()))
                .Generate(5)
                .ToList();
            Stores.AddRange(stores);
            SaveChanges();

            var users = new Faker<User>("de")
                .CustomInstantiator(f => new User(
                    username: f.Internet.UserName(),
                    salt: Convert.ToBase64String(f.Random.Bytes(32)),
                    passwordHash: Convert.ToBase64String(f.Random.Bytes(64)),
                    store: f.Random.ListItem(stores)))
                .Generate(5)
                .DistinctBy(u => u.Store.Id)
                .ToList();
            Users.AddRange(users);
            SaveChanges();

            var offers = new Faker<Offer>("de")
                .CustomInstantiator(f => new Offer(
                    product: f.Random.ListItem(products),
                    store: f.Random.ListItem(stores),
                    price: Math.Round(f.Random.Decimal(100, 999), 2),
                    lastUpdate: new DateTime(2021, 1, 1).AddSeconds(f.Random.Int(0, 180 * 86400))))
                .Generate(125)
                .DistinctBy(o => new { o.Store.Id, o.Product.Ean })
                .ToList();
            Offers.AddRange(offers);
            SaveChanges();
        }
    }
}
