using EfCoreInheritance.Application.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace EfCoreInheritance.Test;

public class UniversityContextTests
{
    private UniversityContext GetDatabase()
    {
        //if (File.Exists("test.db")) File.Delete("test.db");
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var opt = new DbContextOptionsBuilder()
            .UseSqlite(connection)  // Keep connection open (only needed with SQLite in memory db)
            .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;
        var db = new UniversityContext(opt);
        Debug.WriteLine(db.Database.GenerateCreateScript());
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public void CreateDatabaseSuccessTest()
    {
        using var db = GetDatabase();
        Assert.True(db.Database.CanConnect());
    }

    [Fact]
    public void AddEnrollmentTest()
    {
        throw new NotImplementedException();
    }
}