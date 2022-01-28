using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CodeFirstDemo.Application.Infrastructure;
using Bogus;
using CodeFirstDemo.Application.Model;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace CodeFirstDemo.Test
{
    // A file database does not support parallel test execution.
    [Collection("Sequential")]
    public class StoreContextTests
    {
        private StoreContext GetContext()
        {
            var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
            keepAliveConnection.Open();

            var opt = new DbContextOptionsBuilder()
                // For MySQL (requires NuGet Pomelo.EntityFrameworkCore.MySql):
                //    .UseMySql(@"server=localhost;database=Stores;user=root",
                //        new MariaDbServerVersion(new Version(10, 4, 22)))
                // For SQL Server (LocalDB) (requires NuGet Microsoft.EntityFrameworkCore.SqlServer):
                //    .UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Stores")
                // For SQLite (requires NuGet Microsoft.EntityFrameworkCore.Sqlite):
                //    .UseSqlite("Data Source=mydb.db")
                .UseSqlite(keepAliveConnection)  // Keep connection open (only needed with SQLite in memory db)
                .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

            var db = new StoreContext(opt);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            return db;
        }
        [Fact]
        public void CreateDatabaseTest()
        {
            using var db = GetContext();
        }

        [Fact]
        public void SeedTest()
        {
            using var db = GetContext();

            Randomizer.Seed = new Random(1335);
            var productCategories = new Faker<ProductCategory>("de")
                .CustomInstantiator(f => new ProductCategory(name: f.Commerce.ProductAdjective())
                {
                    NameEn = f.Commerce.ProductAdjective().OrNull(f, 0.5f)
                })
                .Generate(5)
                .ToList();
            db.ProductCategories.AddRange(productCategories);
            db.SaveChanges();

            var products = new Faker<Product>("de")
                .CustomInstantiator(f => new Product(
                    ean: f.Random.Int(1000, 9999),
                    name: f.Commerce.ProductName(),
                    productCategory: f.Random.ListItem(productCategories)))
                .Generate(25)
                .GroupBy(p => p.Ean)
                .Select(g => g.First())
                .ToList();
            db.Products.AddRange(products);
            db.SaveChanges();

            var stores = new Faker<Store>("de")
                .CustomInstantiator(f => new Store(
                    name: f.Company.CompanyName()))
                .Generate(5)
                .ToList();
            db.Stores.AddRange(stores);
            db.SaveChanges();

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
            db.Offers.AddRange(offers);
            db.SaveChanges();

            // Clear all objects in memory, forces reading the database.
            db.ChangeTracker.Clear();  
            Assert.True(db.ProductCategories.ToList().Count > 0);
            Assert.True(db.Products.ToList().Count > 0);
            Assert.True(db.Stores.ToList().Count > 0);
            Assert.True(db.Offers.ToList().Count > 0);
        }
    }
}
