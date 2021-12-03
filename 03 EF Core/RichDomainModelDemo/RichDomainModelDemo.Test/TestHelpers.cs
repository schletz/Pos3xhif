using Microsoft.EntityFrameworkCore;
using RichDomainModelDemo.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RichDomainModelDemo.Test
{
    public static class TestHelpers
    {
        public static StoreContext GetDbContext(bool deleteDB = false)
        {
            var opt = new DbContextOptionsBuilder()
                .UseSqlite("Data Source=Stores.db");
            if (deleteDB)
            {
                using (var db = new StoreContext(opt.Options))
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            }
            return new StoreContext(opt
                .UseLazyLoadingProxies()
                .LogTo((message) => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                .Options);
        }
    }
}
