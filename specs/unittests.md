# Guideline für Unittests für EF Core und Services

Die folgende Guideline zeigt Beispiele für automatisierte Tests bei EF Core und Serviceübungen.

Folgende Punkte gelten immer:

- Application enthält Stubs (Signaturen + `throw new NotImplementedException()`), damit die
  Tests sofort kompilieren und `dotnet test` den Fortschritt zeigt.
- **Keine Musterlösung im Repo.** Erwartete Werte auf anderem Weg ermitteln (z. B. SQL statt
  LINQ, von Hand nachrechenbare Literale), nie eine Referenzimplementierung in den Tests.
- Reflection prüft die Struktur, wo es das Lernziel verlangt (immutable, kein Default-
  Konstruktor, Property-Typen, abstrakte Member) – bewusst anders als Unittests in der Praxis.
- Tests deterministisch. Die Angabe (README) sagt: Tests nicht ändern, öffentliche Signaturen
  bleiben.
- Verwendung der SQLite in-memory Datenbank ("DataSource=:memory:")
- Wenn EF Core die Connection selbst verwaltet, würde sie nach `EnsureCreated` geschlossen werden.
  Daher wird die Connection zur in-memory Datenbank vorher hergestellt und übergeben.
- Die Testklasse implementiert daher `IDisposable`.
- Beachte, dass die Tests vom Testrunner parallel ausgeführt werden. Zugriff auf Filedatenbanken ist daher kritisch.

## Unittest für Übungen, die mit EF Core eine Datenbank erzeugen

- Es gibt immer 2 Tests:
    - **T00_CanCreateDatabaseTest:** Ein Test, der bestimmt, ob die Datenbank mit `EnsureCreated` überhaupt erzeugt werden kann.
    - **T00_SchemaTest:** Ein Test, der das Schema unter Regelbedingungen befüllt (Happy Path).

- Der Rest der Tests dient zur automatischen Bewertung.
- Der DisplayName wird aus dem Bewertungsraster der Aufgabe gelesen.
- Die Tests sind so aufgebaut, dass Fehler bei der Groß/Kleinschreibung der Properties toleriert werden.
- Damit das Projekt kompiliert, wird über Reflection auf Properties zugegriffen. Die Vorlage der Übungen beinhaltet nur die leere Klassendefinition der Modelklassen.
- Damit die Studenten keine LINQ Statements abschreiben können, wird über SQL geprüft.
- Die Tests sollen nur den definierten Punkt im Bewertungsraster testen und möglichst tolerant gegenüber anderen Implementierungsfehlern sein. Deswegen wird oft ohne Fremdschlüssel-Check gearbeitet.

**GradingTests.cs**
```
// *************************************************************************************************
// Bewertungstests zur automatisierten Kontrolle (Teilaufgabe 1).
// Die DisplayNames entsprechen den Kriterien im Bewertungsschema (bewertung.adoc).
// DO NOT TOUCH
// *************************************************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aufgabe1.Infrastructure;
using Aufgabe1.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Test;

public class GradingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RegistrationContext _db;
    public GradingTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new RegistrationContext(options);
        _db.Database.EnsureCreated();
    }
    // ===== Setup =====

    [Fact(DisplayName = "Setup 01: T00_CanCreateDatabaseTest: Die Datenbank kann mit EnsureCreated erzeugt werden.")]
    public void T00_CanCreateDatabaseTest()
    {
        string createScript = _db.Database.GenerateCreateScript();
        Debug.Write(createScript);

        using var command = _connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table';";
        var result = (long?)command.ExecuteScalar();
        Assert.True(result >= 3, $"Less than 3 tables found. Check your DbSets & mappings.");
    }

    [Fact(DisplayName = "Setup 02: T00_SchemaTest: Alle Tabellen können unter Regelbedingungen befüllt werden.")]
    public void T00_SchemaTest() => InsertRow(
        "INSERT INTO Game(Name, PrizePool_MinAmount, PrizePool_MaxAmount) VALUES ('Counter-Strike 2', 10000, 50000);",
        "INSERT INTO Game(Name, PrizePool_MinAmount, PrizePool_MaxAmount) VALUES ('Valorant', 5000, 20000);",
        "INSERT INTO Person(Firstname, Lastname, PersonType, Gamertag, SkillRating, Country) VALUES ('Mara', 'Stein', 'Player', 'm4r4', 2500, 'AT');",
        "INSERT INTO Person(Firstname, Lastname, PersonType, Gamertag, SkillRating, Country) VALUES ('Nora', 'Funk', 'Player', 'n0ra', 3100, NULL);",
        "INSERT INTO Person(Firstname, Lastname, PersonType, Email, OrganizationName) VALUES ('Leo', 'Berg', 'Organizer', 'leo@league.at', 'ESL');",
        "INSERT INTO Registration(PlayerId, GameId, RegisteredAt) VALUES (1, 1, '2026-07-01T10:00:00');",
        "INSERT INTO Registration(PlayerId, GameId, RegisteredAt) VALUES (2, 2, '2026-07-02T10:00:00');",
        "INSERT INTO QualificationMatch(RegistrationId, MatchDate, RefereeId, Tier, Notes) VALUES (1, '2026-07-05T18:00:00', 3, 'S', 'solid aim');",
        "INSERT INTO QualificationMatch(RegistrationId, MatchDate, RefereeId, Tier, Notes) VALUES (2, '2026-07-06T18:00:00', 3, NULL, NULL);",
        "INSERT INTO Platform(GameId, Name, ReleaseYear) VALUES (1, 'PC', 2020);",
        "INSERT INTO Platform(GameId, Name, ReleaseYear) VALUES (1, 'PlayStation 5', 2020);");

    // ===== Entity mapping (Kriterium 01-06) =====

    [Fact(DisplayName = "Kriterium 01 (1 Pkt): Das Entity Game wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K01_GameEntityIsMapped() => InsertRow(
        "INSERT INTO Game(Name, PrizePool_MinAmount, PrizePool_MaxAmount) VALUES ('Counter-Strike 2', 10000, 50000);");

    [Fact(DisplayName = "Kriterium 02 (1 Pkt): Das Entity Person wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K02_PersonEntityIsMapped() => InsertRow(
        "INSERT INTO Person(Firstname, Lastname, PersonType) VALUES ('Mara', 'Stein', 'Player');");

    [Fact(DisplayName = "Kriterium 03 (1 Pkt): Das Entity Player wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K03_PlayerEntityIsMapped() => InsertRow(
        "INSERT INTO Person(Firstname, Lastname, PersonType, Gamertag, SkillRating, Country) VALUES ('Mara', 'Stein', 'Player', 'm4r4', 2500, 'AT');");

    [Fact(DisplayName = "Kriterium 04 (1 Pkt): Das Entity Organizer wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K04_OrganizerEntityIsMapped() => InsertRow(
        "INSERT INTO Person(Firstname, Lastname, PersonType, Email, OrganizationName) VALUES ('Leo', 'Berg', 'Organizer', 'leo@league.at', 'ESL');");

    [Fact(DisplayName = "Kriterium 05 (1 Pkt): Das Entity Registration wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K05_RegistrationEntityIsMapped() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO Registration(PlayerId, GameId, RegisteredAt) VALUES (1, 1, '2026-07-01T10:00:00');");

    [Fact(DisplayName = "Kriterium 06 (1 Pkt): Das Entity QualificationMatch wird mit den definierten Feldern in der Datenbank abgebildet.")]
    public void K06_QualificationMatchEntityIsMapped() => InsertRow(foreignKeyCheck: false,
        "INSERT INTO QualificationMatch(RegistrationId, MatchDate, RefereeId, Tier, Notes) VALUES (1, '2026-07-05T18:00:00', 1, NULL, NULL);");

    // ===== Relationships and constraints (Kriterium 07-09) =====

    [Fact(DisplayName = "Kriterium 07 (1 Pkt): Die 1:0..1 Beziehung zwischen Registration und QualificationMatch ist korrekt konfiguriert.")]
    public void K07_RegistrationToQualificationMatchIsOneToOne()
    {
        var principal = _db.Model.GetEntityByClassname("Registration");
        var dependent = _db.Model.GetEntityByClassname("QualificationMatch");
        var fk = dependent.FindDeclaredNavigationIgnoreCase("Registration")?.ForeignKey;
        Assert.NotNull(fk);
        Assert.True(fk!.IsUnique, "The foreign key to Registration must be unique (1:0..1).");
        Assert.True(fk.DeclaringEntityType == dependent);
        Assert.True(fk.PrincipalEntityType == principal);
    }

    [Fact(DisplayName = "Kriterium 08 (1 Pkt): Player.gamertag ist unique")]
    public void K08_GamertagIsUnique() => InsertRowShouldFail(foreignKeyCheck: false,
        "INSERT INTO Person(Firstname, Lastname, PersonType, Gamertag, SkillRating) VALUES ('Mara', 'Stein', 'Player', 'dup', 2500);",
        "INSERT INTO Person(Firstname, Lastname, PersonType, Gamertag, SkillRating) VALUES ('Nora', 'Funk', 'Player', 'dup', 3100);");

    [Fact(DisplayName = "Kriterium 09 (1 Pkt): Das Tupel Registration.player und Registration.game ist unique.")]
    public void K09_RegistrationPlayerAndGameIsUnique() => InsertRowShouldFail(foreignKeyCheck: false,
        "INSERT INTO Registration(PlayerId, GameId, RegisteredAt) VALUES (1, 1, '2026-07-01T10:00:00');",
        "INSERT INTO Registration(PlayerId, GameId, RegisteredAt) VALUES (1, 1, '2026-07-02T10:00:00');");

    // ===== Value objects, rich type, discriminator, converter (Kriterium 10-14) =====

    [Fact(DisplayName = "Kriterium 10 (1 Pkt): Game.prizePool ist als value object definiert.")]
    public void K10_PrizePoolIsValueObject()
    {
        Assert.True(_db.Model.GetEntityByClassname("PrizePool")
            .IsInOwnershipPath(_db.Model.GetEntityByClassname("Game")));
        EnsureConstraint("Game", "PrizePool_MinAmount", "TEXT");
        EnsureConstraint("Game", "PrizePool_MaxAmount", "TEXT");
    }

    [Fact(DisplayName = "Kriterium 11 (1 Pkt): Game.availablePlatforms ist als Liste von value objects definiert.")]
    public void K11_AvailablePlatformsIsListOfValueObjects()
    {
        var platform = _db.Model.GetEntityByClassname("Platform");
        Assert.True(platform.IsInOwnershipPath(_db.Model.GetEntityByClassname("Game")));
        Assert.Equal("Id", platform.FindPrimaryKey()!.Properties.Single().Name, StringComparer.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Kriterium 12 (1 Pkt): Player.skillRating ist als rich type mit converter definiert.")]
    public void K12_SkillRatingIsRichType()
    {
        EnsureConstraint("Person", "SkillRating", "INTEGER");
        Assert.Equal("SkillRating", typeof(Player).GetPropertyIgnoreCase("SkillRating")?.PropertyType.Name);
        var converter = _db.Model.GetEntityByClassname("Player").GetPropertyIgnoreCase("SkillRating").GetValueConverter();
        Assert.NotNull(converter);
    }

    [Fact(DisplayName = "Kriterium 13 (1 Pkt): Person.personType ist als Discriminator definiert.")]
    public void K13_PersonHasDiscriminator()
    {
        Assert.Equal("PersonType", _db.Model.GetEntityByClassname("Person").GetDiscriminatorPropertyName(), StringComparer.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "Kriterium 14 (1 Pkt): QualificationMatch.tier wird mit den korrekten Werten (S, A, B und C) gespeichert.")]
    public void K14_TierIsStoredWithCorrectValues()
    {
        EnsureConstraint("QualificationMatch", "Tier", "TEXT");
        var converter = _db.Model.GetEntityByClassname("QualificationMatch").GetPropertyIgnoreCase("Tier").GetValueConverter();
        Assert.NotNull(converter);
        Assert.Equal("PerformanceTier", converter!.ConvertFromProvider("S")?.GetType().Name);
        Assert.Equal("PerformanceTier", converter.ConvertFromProvider("A")?.GetType().Name);
        Assert.Equal("PerformanceTier", converter.ConvertFromProvider("B")?.GetType().Name);
        Assert.Equal("PerformanceTier", converter.ConvertFromProvider("C")?.GetType().Name);
    }

    // ===== Validation of the shared types (Kriterium 15-16) =====

    [Fact(DisplayName = "Kriterium 15 (1 Pkt): Die Klasse PrizePool wirft eine ArgumentException bei ungültigen Argumenten.")]
    public void K15_PrizePoolThrowsOnInvalidArguments()
    {
        var type = typeof(PrizePool);
        Assert.Equal(typeof(decimal), type.GetPropertyIgnoreCase("MinAmount")?.PropertyType);
        Assert.Equal(typeof(decimal), type.GetPropertyIgnoreCase("MaxAmount")?.PropertyType);
        var ex = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, 5000M, 1000M));
        Assert.True(ex.InnerException is ArgumentException);
    }

    [Fact(DisplayName = "Kriterium 16 (1 Pkt): Die Klasse SkillRating wirft eine ArgumentException bei ungültigen Argumenten.")]
    public void K16_SkillRatingThrowsOnInvalidArguments()
    {
        var type = typeof(SkillRating);
        Assert.Equal(typeof(int), type.GetPropertyIgnoreCase("Value")?.PropertyType);
        var ex = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, -1));
        Assert.True(ex.InnerException is ArgumentException);
        ex = Assert.Throws<TargetInvocationException>(() => Activator.CreateInstance(type, 5001));
        Assert.True(ex.InnerException is ArgumentException);
    }

    private void InsertRowShouldFail(bool foreignKeyCheck, params string[] commandTexts) => InsertRow(true, foreignKeyCheck, commandTexts);
    private void InsertRow(params string[] commandTexts) => InsertRow(false, true, commandTexts);
    private void InsertRow(bool foreignKeyCheck, params string[] commandTexts) => InsertRow(false, foreignKeyCheck, commandTexts);

    private void InsertRow(bool shouldFail, bool foreignKeyCheck, params string[] commandTexts)
    {
        bool failed = false;
        using (var command = _connection.CreateCommand())
        {
            command.CommandText = $"PRAGMA foreign_keys = {(foreignKeyCheck ? 1 : 0)}";
            command.ExecuteNonQuery();
        }

        foreach (var commandText in commandTexts)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = commandText;
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
            Assert.Fail($"Query should fail, but it didn't. {string.Join(Environment.NewLine, commandTexts)}");
    }

    private void EnsureConstraint(string table, string column, string type, bool isPk = false)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = $"PRAGMA table_info({table})";
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

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}

public static class IModelExtensions
{
    public static IEntityType GetEntityByClassname(this IModel model, string name) =>
        model.GetEntityTypes().FirstOrDefault(t => t.ClrType.Name == name)
        ?? throw new ArgumentException($"Entity {name} not found in Model.");

    /// <summary>
    /// Looks up an EF Core property by name, ignoring case, so that student models with
    /// inconsistent casing (e.g. "minAmount" instead of "MinAmount") still work.
    /// </summary>
    public static IProperty GetPropertyIgnoreCase(this IEntityType entityType, string name) =>
        entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        ?? throw new ArgumentException($"Property {name} not found on entity {entityType.ClrType.Name}.");

    /// <summary>
    /// Looks up a declared EF Core navigation by name, ignoring case, so that student models
    /// with inconsistent casing (e.g. "qualificationMatch" instead of "QualificationMatch") still work.
    /// </summary>
    public static INavigation? FindDeclaredNavigationIgnoreCase(this IEntityType entityType, string name) =>
        entityType.GetDeclaredNavigations().FirstOrDefault(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

public static class ReflectionExtensions
{
    /// <summary>
    /// Looks up a public instance property by name via reflection, ignoring case, so that
    /// student classes with inconsistent casing (e.g. "minAmount" instead of "MinAmount") still work.
    /// </summary>
    public static PropertyInfo? GetPropertyIgnoreCase(this Type type, string name) =>
        type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

    /// <summary>
    /// Reads a public instance property's value by name, ignoring case.
    /// </summary>
    public static object? GetPropertyValueIgnoreCase(this object obj, string name) =>
        (obj.GetType().GetPropertyIgnoreCase(name)
            ?? throw new ArgumentException($"Property {name} not found on type {obj.GetType().Name}."))
        .GetValue(obj);
}
```

## Unittest für ein Service

Soll im Rahmen eines Übungsbeispieles eine Serviceklasse geschrieben werden, die über DI den Datenbankcontext bekommt, soll diese Testvorlage verwendet werden.
Die Methoden werden mit SQL geprüft, damit die Studenten nicht die LINQ Statements aus dem Test abschreiben können.

Der auskommentierte Test `ExportDatabaseTest` exportiert die Datenbank als TSV.
Hierfür gibt es im DbContext die Methode `WriteToTsv`.

**GradingTests.cs**
```
// *************************************************************************************************
// Bewertungstests zur automatisierten Kontrolle (Teilaufgabe 2).
// Die DisplayNames entsprechen den Kriterien im Bewertungsschema (bewertung.adoc).
// Die erwarteten Werte werden ausschließlich über SQL ermittelt, damit die LINQ-Lösung der
// Serviceaufgaben nicht aus dieser Testklasse abgelesen werden kann.
// DO NOT TOUCH
// *************************************************************************************************
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using Aufgabe2;
using BaseModel.Infrastructure;
using BaseModel.Model;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public class GradingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DonationContext _db;
    private readonly DonationService _service;

    public GradingTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new DonationContext(options);
        _db.Database.EnsureCreated();
        _db.Seed();
        _service = new DonationService(_db);
    }

    //[Fact(DisplayName = "Setup: Export database to tsv.")]
    //public void ExportDatabaseTest() => _db.WriteToTsv("../../../../../datafiles");

    [Fact(DisplayName = "Kriterium 01 (2 Pkt): Die Methode GetCategoryFunding ist korrekt (inkl. absteigender Sortierung).")]
    public void K01_GetCategoryFunding()
    {
        // Name, sum of all confirmed donations across the category's projects (0 if none)
        // and the total number of projects in the category.
        var sql = @"
            SELECT c.name AS name,
                   COALESCE((SELECT SUM(CAST(d.amount AS REAL))
                             FROM donation d
                             JOIN project p ON p.id = d.projectId
                             JOIN donationStatus s ON s.id = d.statusId
                             WHERE p.categoryId = c.id AND s.name = 'Confirmed'), 0.0) AS totalAmount,
                   (SELECT COUNT(*) FROM project p WHERE p.categoryId = c.id) AS projectCount
            FROM category c";

        var expected = QueryDatabase(sql, r =>
            new CategoryFunding(r.GetString(0), (decimal)r.GetDouble(1), r.GetInt32(2)));

        var actual = _service.GetCategoryFunding();

        AssertSameSet(expected, actual);
        for (int i = 1; i < actual.Count; i++)
            Assert.True(actual[i - 1].TotalAmount >= actual[i].TotalAmount,
                "GetCategoryFunding must be ordered by the total amount, descending.");
    }

    [Fact(DisplayName = "Kriterium 02 (2 Pkt): Die Methode GetSupportersForCategory ist korrekt.")]
    public void K02_GetSupportersForCategory()
    {
        foreach (var category in new[] { "Environment", "Culture", "Sport", "Social", "Health", "DoesNotExist" })
        {
            var sql = $@"
                SELECT s.id AS id
                FROM supporter s
                WHERE EXISTS (SELECT 1
                              FROM donation d
                              JOIN donationStatus st ON st.id = d.statusId
                              JOIN project p ON p.id = d.projectId
                              JOIN category c ON c.id = p.categoryId
                              WHERE d.supporterId = s.id
                                AND st.name = 'Confirmed'
                                AND c.name = '{category}')";

            var expectedIds = QueryDatabase(sql, r => r.GetInt32(0)).OrderBy(id => id).ToList();
            var actualIds = _service.GetSupportersForCategory(category)
                .Select(s => s.Id)
                .OrderBy(id => id)
                .ToList();

            Assert.True(expectedIds.SequenceEqual(actualIds),
                $"GetSupportersForCategory(\"{category}\") failed. Expected ids [{string.Join(",", expectedIds)}], got [{string.Join(",", actualIds)}].");
        }
    }

    [Fact(DisplayName = "Kriterium 03 (2 Pkt): Die Methode GetUnderfundedProjects ist korrekt.")]
    public void K03_GetUnderfundedProjects()
    {
        foreach (var maxPercent in new[] { 25m, 50m, 100m })
        {
            // Open projects whose confirmed donations are below goalAmount * maxPercent / 100.
            var percent = maxPercent.ToString(CultureInfo.InvariantCulture);
            var sql = $@"
                SELECT p.title AS title,
                       CAST(p.goalAmount AS REAL) AS goalAmount,
                       COALESCE((SELECT SUM(CAST(d.amount AS REAL))
                                 FROM donation d
                                 JOIN donationStatus s ON s.id = d.statusId
                                 WHERE d.projectId = p.id AND s.name = 'Confirmed'), 0.0) AS collected
                FROM project p
                WHERE p.isOpen = 1
                  AND COALESCE((SELECT SUM(CAST(d.amount AS REAL))
                                FROM donation d
                                JOIN donationStatus s ON s.id = d.statusId
                                WHERE d.projectId = p.id AND s.name = 'Confirmed'), 0.0)
                      < CAST(p.goalAmount AS REAL) * {percent} / 100.0";

            var expected = QueryDatabase(sql, r =>
            {
                var goal = (decimal)r.GetDouble(1);
                var collected = (decimal)r.GetDouble(2);
                return new ProjectProgress(r.GetString(0), goal, collected, goal - collected);
            });

            var actual = _service.GetUnderfundedProjects(maxPercent);
            AssertSameSet(expected, actual);
        }
    }

    [Fact(DisplayName = "Kriterium 04 (1 Pkt): Die Methode AddDonation prüft supporterId korrekt.")]
    public void K04_AddDonationChecksSupporter()
    {
        var project = _db.Projects.First(p => p.IsOpen);
        var ex = Assert.Throws<DonationException>(
            () => _service.AddDonation(999999, project.Id, 100m, new DateOnly(2026, 5, 1)));
        Assert.Contains("supporter", ex.Message.ToLower());
    }

    [Fact(DisplayName = "Kriterium 05 (1 Pkt): Die Methode AddDonation prüft projectId korrekt.")]
    public void K05_AddDonationChecksProject()
    {
        var supporter = _db.Supporters.First();
        var ex = Assert.Throws<DonationException>(
            () => _service.AddDonation(supporter.Id, 999999, 100m, new DateOnly(2026, 5, 1)));
        Assert.Contains("project", ex.Message.ToLower());
    }

    [Fact(DisplayName = "Kriterium 06 (1 Pkt): Die Methode AddDonation prüft isOpen korrekt.")]
    public void K06_AddDonationChecksIsOpen()
    {
        var supporter = _db.Supporters.First();
        var closedProject = _db.Projects.First(p => !p.IsOpen);
        var ex = Assert.Throws<DonationException>(
            () => _service.AddDonation(supporter.Id, closedProject.Id, 100m, new DateOnly(2026, 5, 1)));
        Assert.Contains("closed", ex.Message.ToLower());
    }

    [Fact(DisplayName = "Kriterium 07 (3 Pkt): Die Methode AddDonation erstellt eine korrekte Spende und schließt das Projekt bei Erreichen des Spendenziels.")]
    public void K07_AddDonationCreatesAndCloses()
    {
        // Fresh, fully controlled project (goal 1000, no donations yet).
        var category = _db.Categories.First();
        var project = new Project("Goal Test", category, 1000m, new DateOnly(2026, 12, 31), isOpen: true);
        var supporterA = new Supporter("goalA", "a@test.at");
        var supporterB = new Supporter("goalB", "b@test.at");
        _db.AddRange(project, supporterA, supporterB);
        _db.SaveChanges();

        // A normal donation below the goal: must be created correctly and keep the project open.
        var donation = _service.AddDonation(supporterA.Id, project.Id, 100m, new DateOnly(2026, 5, 1));
        Assert.True(donation.Id != 0);

        _db.ChangeTracker.Clear();
        var stored = _db.Donations
            .Include(d => d.Status).Include(d => d.Supporter).Include(d => d.Project)
            .First(d => d.Id == donation.Id);
        Assert.Equal(100m, stored.Amount);
        Assert.Equal("Confirmed", stored.Status.Name);
        Assert.Equal(supporterA.Id, stored.Supporter.Id);
        Assert.Equal(project.Id, stored.Project.Id);
        Assert.True(_db.Projects.First(p => p.Id == project.Id).IsOpen, "Project must stay open below the goal.");

        // A donation that reaches the goal must close the project.
        _service.AddDonation(supporterB.Id, project.Id, 1000m, new DateOnly(2026, 5, 2));
        _db.ChangeTracker.Clear();
        Assert.False(_db.Projects.First(p => p.Id == project.Id).IsOpen,
            "Project must be closed once the goal amount is reached.");
    }

    private static void AssertSameSet<T>(IReadOnlyCollection<T> expected, IReadOnlyCollection<T> actual)
    {
        Assert.True(expected.Count == actual.Count,
            $"Expected {expected.Count} rows, got {actual.Count}.");
        foreach (var item in expected)
            Assert.True(actual.Contains(item), $"Missing row in your result: {item}.");
    }

    private List<T> QueryDatabase<T>(string commandText, Func<DbDataReader, T> projection)
    {
        var database = _db.Database;
        using var command = database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;
        database.OpenConnection();
        using var reader = command.ExecuteReader();
        var rows = new List<T>();
        while (reader.Read())
            rows.Add(projection(reader));
        return rows;
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }
}
```

**DonationContext**
```
public class DonationContext : DbContext
{
   // DBSets ...

   public void WriteToTsv(string path)
   {
       File.WriteAllText(
           Path.Combine(path, "dump.sql"),
           Database.GenerateCreateScript(),
           new UTF8Encoding(false, false));
       WriteEntitiesToTsv("category", path);
       WriteEntitiesToTsv("donationStatus", path);
       WriteEntitiesToTsv("supporter", path);
       WriteEntitiesToTsv("manager", path);
       WriteEntitiesToTsv("project", path);
       WriteEntitiesToTsv("donation", path);
   }

   private void WriteEntitiesToTsv(string tableName, string path)
   {
       var encoder = new UTF8Encoding(false, false);
       var config = new CsvConfiguration(CultureInfo.InvariantCulture)
       {
           Delimiter = "\t",
           NewLine = "\r\n",
       };

       using var command = Database.GetDbConnection().CreateCommand();
       command.CommandText = $"SELECT * FROM {tableName}";
       Database.OpenConnection();

       using var reader = command.ExecuteReader();
       using var writer = new StreamWriter(Path.Combine(path, $"{tableName.ToLower()}.tsv"), false, encoder);
       using var csv = new CsvWriter(writer, config);

       for (int i = 0; i < reader.FieldCount; i++)
           csv.WriteField(reader.GetName(i));
       csv.NextRecord();

       while (reader.Read())
       {
           for (int i = 0; i < reader.FieldCount; i++)
           {
               var value = reader.GetValue(i);
               if (value == DBNull.Value)
                   csv.WriteField(string.Empty);
               else if (value is DateTime dt)
                   csv.WriteField(dt.ToString("s"));
               else if (value is DateOnly d)
                   csv.WriteField(d.ToString("yyyy-MM-dd"));
               else
                   csv.WriteField(value);
           }
           csv.NextRecord();
       }
   }
}
```