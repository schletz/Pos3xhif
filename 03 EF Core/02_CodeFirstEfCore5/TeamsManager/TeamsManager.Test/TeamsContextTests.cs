using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TeamsManager.Application.Infrastructure;

namespace TeamsManager.Test;

public class TeamsContextTests
{
    private TeamsContext GetDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var opt = new DbContextOptionsBuilder()
            .UseSqlite(connection)  // Keep connection open (only needed with SQLite in memory db)
            .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new TeamsContext(opt);
        db.Database.EnsureCreated();
        return db;
    }
    [Fact]
    public void CreateDatabaseSuccessTest()
    {
        using var db = GetDatabase();
    }

    [Fact]
    public void AddHandinSuccessTest()
    {
        // TODO: Add your implementation
        throw new NotImplementedException();
    }

    [Fact]
    public void AddTeamWithSameNameThrowsDbUpdateExceptionTest()
    {
        // TODO: Add your implementation
        throw new NotImplementedException();
    }
}