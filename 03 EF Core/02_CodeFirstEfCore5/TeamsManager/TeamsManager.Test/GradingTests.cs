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
        Assert.True(result >= 3, $"Less than 3 Tables found. Check your DbSets.");
    }

    [Fact]
    public void T01_InsertTeacherTest() => InsertRow(
        "INSERT INTO Teacher (Firstname, Lastname, Email) VALUES ('first', 'last', 'first@last.at')");

    [Fact]
    public void T02_InsertStudentTest() => InsertRow(
        "INSERT INTO Student (Firstname, Lastname, Email) VALUES ('first', 'last', 'first@last.at')");

    [Fact]
    public void T03_InsertTeamTest() => InsertRow(
        "INSERT INTO Team (Name, Schoolclass) VALUES ('team', '5CAIF')");

    [Fact]
    public void T04_TeamNameIsUniqueTest()
    {
        // Zuerst testen, ob überhaupt ein INSERT möglich ist.
        // Sonst läuft der Test durch, wenn ein anderer Fehler (keine Table) geliefert wird.
        InsertRow("INSERT INTO Team (Name, Schoolclass) VALUES ('team', '5CAIF')");
        InsertRowShouldFail(
            "INSERT INTO Team (Name, Schoolclass) VALUES ('team', '5CAIF')",
            "INSERT INTO Team (Name, Schoolclass) VALUES ('team', '5CAIF')");
    }

    [Fact]
    public void T05_InsertTaskTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Task (Subject, Title, TeamId, TeacherId, ExpirationDate, MaxPoints) VALUES ('subject', 'title', 1, 1, '2025-10-18T14:00:00Z', 24)");

    [Fact]
    public void T06_MaxPointsInTaskIsNullableTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Task (Subject, Title, TeamId, TeacherId, ExpirationDate) VALUES ('subject', 'title', 1, 1, '2025-10-18T14:00:00Z')");

    [Fact]
    public void T07_InsertHandinTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Handin (TaskId, StudentId, Date, ReviewDate, Points) VALUES (1, 1, '2025-10-15T15:00:00Z', '2025-10-19T15:00:00Z', 14)");

    [Fact]
    public void T08_ReviewDateInHandinIsNullableTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Handin (TaskId, StudentId, Date, Points) VALUES (1, 1, '2025-10-15T15:00:00Z', 14)");

    [Fact]
    public void T09_PointsInHandinIsNullableTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Handin (TaskId, StudentId, Date, ReviewDate) VALUES (1, 1, '2025-10-15T15:00:00Z', '2025-10-19T15:00:00Z')");

    private TeamsContext GetEmptyDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .Options;

        var db = new TeamsContext(options);
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
