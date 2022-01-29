using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Bogus;
using CodeFirstDemo.Application.Model;

namespace CodeFirstDemo.Test
{
    public class StoreContextTests : DatabaseTest
    {
        [Fact]
        public void CreateDatabaseTest()
        {
            _db.Database.EnsureCreated();
        }

        [Fact]
        public void SeedTest()
        {
            _db.Database.EnsureCreated();

            Randomizer.Seed = new Random(1335);
            var productCategories = new Faker<ProductCategory>("de")
                .CustomInstantiator(f => new ProductCategory(name: f.Commerce.ProductAdjective())
                {
                    NameEn = f.Commerce.ProductAdjective().OrNull(f, 0.5f)
                })
                .Generate(5)
                .ToList();
            _db.ProductCategories.AddRange(productCategories);
            _db.SaveChanges();

            var products = new Faker<Product>("de")
                .CustomInstantiator(f => new Product(
                    ean: f.Random.Int(1000, 9999),
                    name: f.Commerce.ProductName(),
                    productCategory: f.Random.ListItem(productCategories)))
                .Generate(25)
                .GroupBy(p => p.Ean)
                .Select(g => g.First())
                .ToList();
            _db.Products.AddRange(products);
            _db.SaveChanges();

            var stores = new Faker<Store>("de")
                .CustomInstantiator(f => new Store(
                    name: f.Company.CompanyName()))
                .Generate(5)
                .ToList();
            _db.Stores.AddRange(stores);
            _db.SaveChanges();

            var offers = new Faker<Offer>("de")
                .CustomInstantiator(f => new Offer(
                    product: f.Random.ListItem(products),
                    store: f.Random.ListItem(stores),
                    price: Math.Round(f.Random.Decimal(100, 999), 2),
                    lastUpdate: new DateTime(2021, 1, 1).AddSeconds(f.Random.Int(0, 180 * 86400))))
                .Generate(125)
                .GroupBy(o => new { o.StoreId, o.ProductEan })
                .Select(g => g.First())
                .ToList();
            _db.Offers.AddRange(offers);
            _db.SaveChanges();

            // Clear all objects in memory, forces reading the database.
            _db.ChangeTracker.Clear();
            Assert.True(_db.ProductCategories.ToList().Count > 0);
            Assert.True(_db.Products.ToList().Count > 0);
            Assert.True(_db.Stores.ToList().Count > 0);
            Assert.True(_db.Offers.ToList().Count > 0);
        }
    }
}
