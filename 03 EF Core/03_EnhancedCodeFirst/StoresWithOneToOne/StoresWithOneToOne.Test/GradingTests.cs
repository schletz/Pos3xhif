// *************************************************************************************************
// Grading tests for example.
// DO NOT TOUCH
// *************************************************************************************************
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using TeamsManager.Application.Infrastructure;

namespace TeamsManager.Test;

public class GradingTests
{
    [Fact]
    public void T00_CanCreateDatabaseTest()
    {
        using var db = GetEmptyDbContext();
        string createScript = db.Database.GenerateCreateScript();
        Debug.Write(createScript);

        using var command = db.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table';";
        db.Database.OpenConnection();
        var result = (long?)command.ExecuteScalar();
        Assert.True(result >= 6, $"Less than 6 tables found. Check your DbSets & mappings.");
    }

    // ===== Basic inserts =====

    [Fact]
    public void T01_InsertStoreTest() => InsertRow(
        "INSERT INTO Store (Name) VALUES ('SuperStore')");

    [Fact]
    public void T02_StoreNameUniqueTest()
    {
        InsertRow("INSERT INTO Store (Name) VALUES ('UniqueStore')");
        InsertRowShouldFail(
            "INSERT INTO Store (Name) VALUES ('UniqueStore')",
            "INSERT INTO Store (Name) VALUES ('UniqueStore')");
    }

    [Fact]
    public void T03_InsertProductTest() => InsertRow(
        "INSERT INTO Product (Ean, Name) VALUES (9000000012345, 'Choco Bar')");

    [Fact]
    public void T04_ProductNameUniqueTest()
    {
        InsertRow("INSERT INTO Product (Ean, Name) VALUES (1111, 'OnlyOnce')");
        InsertRowShouldFail(
            "INSERT INTO Product (Ean, Name) VALUES (2222, 'OnlyOnce')",
            "INSERT INTO Product (Ean, Name) VALUES (2222, 'OnlyOnce')");
    }

    [Fact]
    public void T05_ProductEanIsPrimaryKey()
    {
        EnsureConstraint("Product", "Ean", "INTEGER", isPk: true);
    }

    [Fact]
    public void T06_InsertStoreDetail()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO StoreDetail (Address, Zip, City, StoreId) VALUES ('Addr 1', '1010', 'Wien', 1)");
    }

    [Fact]
    public void T07_InsertProductDetail()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO ProductDetail (Description, Manufacturer, ProductEan) VALUES ('Desc', 'Manu', 1)");
    }

    [Fact]
    public void T08_InsertOpeningHour()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO OpeningHour (StoreDetailId, Weekday, 'From', 'To') VALUES (1, 'MO', '08:00:00', '12:00:00')");
    }

    [Fact]
    public void T09_OpeningHour_EnumAsText()
    {
        EnsureConstraint("OpeningHour", "Weekday", "TEXT");
    }

    [Fact]
    public void T10_InsertOffer()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (1, 12345, 1.99, '2025-10-18T14:00:00Z')");
    }

    [Fact]
    public void T11_OfferUniquePerStoreAndProduct()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (1, 10, 0.99, '2025-10-18T14:00:00Z')",
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (1, 11, 0.99, '2025-10-18T14:00:00Z')",
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (2, 11, 0.99, '2025-10-18T14:00:00Z')");

        // Gleiche (StoreId, ProductEan) Kombination -> Unique-Verletzung
        InsertRowShouldFail(
            "INSERT INTO Store (Name) VALUES ('UqOfferStore')",
            "INSERT INTO Product (Ean, Name) VALUES (54321, 'UqOfferProduct')",
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (1, 54321, 1.49, '2025-10-18T15:00:00Z')",
            "INSERT INTO Offer (StoreId, ProductEan, Price, LastUpdate) VALUES (1, 54321, 1.59, '2025-11-18T15:00:00Z')");
    }

    [Fact]
    public void T12_StoreAndStoreDetailIsOneToOne()
    {
        InsertRow(
            "INSERT INTO Store (Name) VALUES ('S1')",
            "INSERT INTO StoreDetail (Address, Zip, City, StoreId) VALUES ('A1','1000','Wien', 1)"
        );
        InsertRowShouldFail(
            "INSERT INTO Store (Name) VALUES ('S1')",
            "INSERT INTO StoreDetail (Address, Zip, City, StoreId) VALUES ('A1','1000','Wien', 1)",
            "INSERT INTO StoreDetail (Address, Zip, City, StoreId) VALUES ('A2','1000','Wien', 1)"
        );
    }

    [Fact]
    public void T13_ProductAndProductDetailIsOneToOne()
    {
        InsertRow(
            "INSERT INTO Product (Ean, Name) VALUES (123, 'Name')",
            "INSERT INTO ProductDetail (Description, Manufacturer, ProductEan) VALUES ('Desc', 'Manu', 123)"
        );
        InsertRowShouldFail(
            "INSERT INTO Product (Ean, Name) VALUES (123, 'Name')",
            "INSERT INTO ProductDetail (Description, Manufacturer, ProductEan) VALUES ('Desc', 'Manu', 123)",
            "INSERT INTO ProductDetail (Description, Manufacturer, ProductEan) VALUES ('Desc2', 'Manu2', 123)"
        );
    }

    private StoreContext GetEmptyDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .Options;

        var db = new StoreContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    private void InsertRowShouldFail(params string[] commandsTexts) => InsertRow(true, true, commandsTexts);
    private void InsertRow(params string[] commandsTexts) => InsertRow(false, true, commandsTexts);
    private void InsertRow(bool foreignKeyCheck, params string[] commandsTexts) => InsertRow(false, foreignKeyCheck, commandsTexts);
    private void InsertRow(bool shouldFail, bool foreignKeyCheck, params string[] commandsTexts)
    {
        using var db = GetEmptyDbContext();
        bool failed = false;
        using (var command = db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = $"PRAGMA foreign_keys = {(foreignKeyCheck ? 1 : 0)}";
            db.Database.OpenConnection();
            command.ExecuteNonQuery();
        }

        foreach (var commandText in commandsTexts)
        {
            using var command = db.Database.GetDbConnection().CreateCommand();
            command.CommandText = commandText;
            db.Database.OpenConnection();
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqliteException e)
            {
                failed = true;
                if (!shouldFail)
                    Assert.Fail($"Query failed: {commandText} with error {e.InnerException?.Message ?? e.Message}");
            }
        }
        if (shouldFail && !failed)
            Assert.Fail($"Query should fail, but it didn't. {string.Join(Environment.NewLine, commandsTexts)}");
    }

    private void EnsureConstraint(string table, string column, string type, bool isPk = false)
    {
        using var db = GetEmptyDbContext();
        using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({table})";
        db.Database.OpenConnection();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string columnName = reader.GetString(1);
            string columnType = reader.GetString(2);
            bool columnPk = reader.GetBoolean(5);
            if (columnName.Equals(column, StringComparison.OrdinalIgnoreCase))
            {
                Assert.True(columnType == type, $"Wrong datatype for {table}.{column}. Expected: {type}, given: {columnType}.");
                Assert.True(columnPk == isPk, $"Wrong primary key constraint {table}.{column}. Expected: {isPk}, given: {columnPk}.");
                return;
            }
        }
        Assert.Fail($"Column {table}.{column} not found.");
    }
}
