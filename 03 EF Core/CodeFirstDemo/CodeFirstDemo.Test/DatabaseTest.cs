using System;
using Microsoft.EntityFrameworkCore;
using CodeFirstDemo.Application.Infrastructure;
using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace CodeFirstDemo.Test
{
    public class DatabaseTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly StoreContext _db;

        public DatabaseTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            // Enable Data Source=stores.db instead of UseSqlite(_connection)
            // if you want to open the database in DBeaver.
            var opt = new DbContextOptionsBuilder()
                //.UseSqlite("Data Source=stores.db") 
                .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
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
