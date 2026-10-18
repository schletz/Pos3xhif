using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TeamsManager.Application.Infrastructure;

namespace TeamsManager.Test;

public class StoreContextTests
{
    private StoreContext GetDatabase()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var opt = new DbContextOptionsBuilder()
            .UseSqlite(connection)  // Keep connection open (only needed with SQLite in memory db)
            .LogTo(message => Debug.WriteLine(message), Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new StoreContext(opt);
        db.Database.EnsureCreated();
        return db;
    }
    [Fact]
    public void CreateDatabaseSuccessTest()
    {
        using var db = GetDatabase();
    }

    /// <summary>
    /// Zeigt, dass nur ein Offer mit gleichem Produkt und Store in der Datenbank angelegt werden kann.
    /// </summary>
    [Fact]
    public void EnsureProductAndStoreInOfferIsUniqueTest()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Zeigt, dass in der Datenbank ein Store samt StoreDetails angelegt werden kann.
    /// </summary>
    [Fact]
    public void InsertStoreAndStoreDetailsTest()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Zeigt, dass in der Datenbank ein Product samt ProductDetails angelegt werden kann.
    /// </summary>
    [Fact]
    public void InsertProductAndProductDetailsTest()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Zeigt, dass ein Store samt OpeningHours angelegt werden kann.
    /// </summary>
    [Fact]
    public void InsertOpeningHourTest()
    {
        throw new NotImplementedException();
    }
}
