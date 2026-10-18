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
            _db.Seed();
            // Clear all objects in memory, forces reading the database.
            _db.ChangeTracker.Clear();
            Assert.True(_db.ProductCategories.Any());
            Assert.True(_db.Products.Any());
            Assert.True(_db.Stores.Any());
            Assert.True(_db.Users.Any());
            Assert.True(_db.Offers.Any());
        }
    }
}
