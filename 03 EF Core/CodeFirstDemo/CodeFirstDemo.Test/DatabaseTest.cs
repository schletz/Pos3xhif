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
            // For MySQL (requires NuGet Pomelo.EntityFrameworkCore.MySql):
            //    .UseMySql(@"server=localhost;database=Stores;user=root",
            //        new MariaDbServerVersion(new Version(10, 4, 22)))
            // For SQL Server (LocalDB) (requires NuGet Microsoft.EntityFrameworkCore.SqlServer):
            //    .UseSqlServer(@"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Stores")
            // For SQLite (requires NuGet Microsoft.EntityFrameworkCore.Sqlite):
            //    .UseSqlite("Data Source=mydb.db")

            var opt = new DbContextOptionsBuilder()
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
