using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using CodeFirstDemo.Application.Infrastructure;

namespace CodeFirstDemo.Test
{
    // A file database does not support parallel test execution.
    [Collection("Sequential")] 
    public class StoreContextTests
    {
        [Fact]
        public void CreateDatabaseTest()
        {
            var opt = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=Stores.db")
                .Options;
            using var db = new StoreContext(opt);
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
