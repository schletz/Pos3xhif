using System;
using Eventmanager.Application.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Eventmanager.Tests;

/// <summary>
/// Base class for tests that need a seeded database. Every test gets its own in-memory
/// SQLite database. The generated SQL statements appear in the output of the test
/// (Test Explorer: "Open additional output for this result").
/// </summary>
public abstract class DatabaseTest : IDisposable
{
    private static readonly object _seedLock = new();
    private readonly SqliteConnection _connection;
    protected readonly EventContext _db;

    protected DatabaseTest()
    {
        // EF Core would close its own connection after EnsureCreated and the in-memory
        // database would be gone. So we open the connection ourselves and pass it.
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder()
            .UseSqlite(_connection)
            .LogTo(
                message => TestContext.Current.TestOutputHelper?.WriteLine(message),
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;
        _db = new EventContext(options);
        _db.Database.EnsureCreated();
        // Bogus uses one static random generator. Tests run in parallel, so without the lock
        // two seeds would draw from the same generator and the data would not be deterministic.
        lock (_seedLock)
        {
            _db.Seed();
        }
        // The seed has filled the change tracker. Tests should start like a fresh request.
        _db.ChangeTracker.Clear();
    }

    /// <summary>
    /// Writes a line to the output of the current test.
    /// </summary>
    protected static void Print(object? value) =>
        TestContext.Current.TestOutputHelper?.WriteLine(value?.ToString() ?? "null");

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
