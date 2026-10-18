using System;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.Data.Sqlite;
using RichDomainModelDemo.Application.Infrastructure;

namespace RichDomainModelDemo.Test
{
    public class DatabaseTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly StoreContext _db;

        public DatabaseTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var opt = new DbContextOptionsBuilder()
                .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
                .UseLazyLoadingProxies()
                .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging()
                .Options;

            _db = new StoreContext(opt);
        }
        public void Dispose()
        {
            _db.Dispose();
            _connection.Dispose();
        }
    }
}
