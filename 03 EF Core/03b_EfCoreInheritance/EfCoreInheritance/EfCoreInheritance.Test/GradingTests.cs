// *************************************************************************************************
// Grading tests for example.
// DO NOT TOUCH
// *************************************************************************************************
using EfCoreInheritance.Application.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Diagnostics;
using System.Linq;

namespace EfCoreInheritance.Test;

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
        Assert.True(result >= 3, $"Less than 3 tables found. Check your DbSets & mappings.");
    }

    [Fact]
    public void T01_InsertCourseTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Course (Title, Credits, CourseCode, InstructorId) VALUES ('Algorithms', 5, 'CS-101', 1)");

    [Fact]
    public void T02_InsertEnrollmentTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Enrollment (EnrolledAt, Grade, StudentId, CourseId) VALUES ('2025-11-08T10:00:00Z', 1.0, 2, 1)");

    [Fact]
    public void T03_InsertInstructorTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Person (Firstname, Lastname, PersonType, EmployeeNo) VALUES ('Ada', 'Lovelace', 'I', 'EMP-1001')");

    [Fact]
    public void T04_InsertStudentTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Person (Firstname, Lastname, PersonType, MatriculationNo, Address_Street, Address_Zip, Address_City) VALUES ('Grace', 'Hopper', 'S', 'MAT-2025-0001', 'Main St 1', '1010', 'Wien')");

    [Fact]
    public void T05_InsertStudentProfileTest() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO StudentProfile (BirthDate, Email, Phone, StudentId) VALUES ('1995-05-23', 'grace.hopper@example.com', '+43 1 2345678', 1)");

    [Fact]
    public void T06_InsertTagTest() => InsertRow(foreignKeyCheck: false,
            "INSERT INTO Tag (Name, Color, CourseId) VALUES ('ComputerScience', 'green', 1)");


    [Fact]
    public void T07_CourseCodeIsUniqueTest()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO Course (Title, Credits, CourseCode, InstructorId) VALUES ('Algorithms', 5, 'CS-101', 1)");
        InsertRowShouldFail(foreignKeyCheck: false,
            "INSERT INTO Course (Title, Credits, CourseCode, InstructorId) VALUES ('Algorithms', 5, 'CS-101', 1)",
            "INSERT INTO Course (Title, Credits, CourseCode, InstructorId) VALUES ('Algorithms', 5, 'CS-101', 1)");
    }

    [Fact]
    public void T08_EmployeeNoIsUniqueTest()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO Person (Firstname, Lastname, PersonType, EmployeeNo) VALUES ('Ada', 'Lovelace', 'I', 'EMP-1001')");
        InsertRowShouldFail(foreignKeyCheck: false,
            "INSERT INTO Person (Firstname, Lastname, PersonType, EmployeeNo) VALUES ('Ada', 'Lovelace', 'I', 'EMP-1001')",
            "INSERT INTO Person (Firstname, Lastname, PersonType, EmployeeNo) VALUES ('Ada', 'Lovelace', 'I', 'EMP-1001')");
    }

    [Fact]
    public void T09_StudentAndStudentProfileIsOneToOneTest()
    {
        InsertRow(foreignKeyCheck: false,
            "INSERT INTO StudentProfile (BirthDate, Email, Phone, StudentId) VALUES ('1995-05-23', 'grace.hopper@example.com', '+43 1 2345678', 1)");
        InsertRowShouldFail(foreignKeyCheck: false,
            "INSERT INTO StudentProfile (BirthDate, Email, Phone, StudentId) VALUES ('1995-05-23', 'grace.hopper@example.com', '+43 1 2345678', 1)",
            "INSERT INTO StudentProfile (BirthDate, Email, Phone, StudentId) VALUES ('1995-05-23', 'grace.hopper@example.com', '+43 1 2345678', 1)");
    }

    [Fact]
    public void T10_PersonHasDiscriminatorPersonTypeTest()
    {
        using var db = GetEmptyDbContext();
        Assert.Equal("PersonType", db.Model.GetEntityByClassname("Person").GetDiscriminatorPropertyName());
    }

    [Fact]
    public void T11_DiscriminatorValuesAreCorrectTest()
    {
        using var db = GetEmptyDbContext();
        Assert.Equal("S", db.Model.GetEntityByClassname("Student").GetDiscriminatorValue());
        Assert.Equal("I", db.Model.GetEntityByClassname("Instructor").GetDiscriminatorValue());
    }

    [Fact]
    public void T12_TagIsValueObjectTest()
    {
        using var db = GetEmptyDbContext();
        Assert.True(db.Model.GetEntityByClassname("Tag").IsInOwnershipPath(db.Model.GetEntityByClassname("Course")));
    }

    [Fact]
    public void T13_AddressIsValueObjectTest()
    {
        using var db = GetEmptyDbContext();
        Assert.True(db.Model.GetEntityByClassname("Address").IsInOwnershipPath(db.Model.GetEntityByClassname("Student")));
    }

    [Fact]
    public void T14_PhoneNumberIsRichTypeTest()
    {
        using var db = GetEmptyDbContext();
        Assert.True(
            db.Model.GetEntityByClassname("StudentProfile").GetProperty("Phone").GetValueConverter()?.ModelClrType.Name == "PhoneNumber",
            "PhoneNumber has no value converter. Did you fake it with a string or an entity??? 😠");
    }


    private UniversityContext GetEmptyDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var options = new DbContextOptionsBuilder()
            .UseSqlite(connection)
            .Options;

        var db = new UniversityContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    private void InsertRowShouldFail(bool foreignKeyCheck, params string[] commandsTexts) => InsertRow(true, foreignKeyCheck, commandsTexts);
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

public static class IModelExtensions
{
    public static IEntityType GetEntityByClassname(this IModel model, string name) =>
        model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name == name) ?? throw new ArgumentException($"Entity {name} not found in Model.");
}
