// *************************************************************************************************
// Tests for BusQueryService. DO NOT CHANGE THIS FILE.
// The expected values are calculated with SQL only, so the LINQ solution cannot be read from
// this file.
// *************************************************************************************************
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using BusQueries.Application.Infrastructure;
using BusQueries.Application.Model;
using BusQueries.Application.Results;
using BusQueries.Application.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BusQueries.Tests;

public class BusQueryServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly BusContext _db;
    private readonly BusQueryService _service;

    public BusQueryServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new BusContext(options);
        _db.Database.EnsureCreated();
        _db.Seed();
        _service = new BusQueryService(_db);
    }

    [Fact(DisplayName = "GetStopsInMunicipality returns the stops of the municipality, ordered by name.")]
    public void GetStopsInMunicipalityTest()
    {
        foreach (var municipality in new[] { "Dornbirn", "Lustenau", "Wien" })
        {
            var expected = QueryDatabase(
                $"SELECT Id FROM Stops WHERE Municipality = '{municipality}' ORDER BY Name",
                r => r.GetInt32(0));

            var actual = _service.GetStopsInMunicipality(municipality).Select(s => s.Id).ToList();

            AssertSameSequence(expected, actual, $"GetStopsInMunicipality(\"{municipality}\")");
        }
    }

    [Fact(DisplayName = "GetLicensePlates returns the license plates of autonomous or conventional vehicles, ordered alphabetically.")]
    public void GetLicensePlatesTest()
    {
        foreach (var autonomous in new[] { true, false })
        {
            var expected = QueryDatabase(
                $"SELECT LicensePlate FROM Vehicles WHERE IsAutonomous = {(autonomous ? 1 : 0)} ORDER BY LicensePlate",
                r => r.GetString(0));

            var actual = _service.GetLicensePlates(autonomous);

            AssertSameSequence(expected, actual, $"GetLicensePlates({autonomous})");
        }
    }

    [Fact(DisplayName = "GetOperatorsWithoutTrips returns only operators without any trip.")]
    public void GetOperatorsWithoutTripsTest()
    {
        var expected = QueryDatabase(@"
            SELECT o.Id FROM SafetyOperators o
            WHERE NOT EXISTS (SELECT 1 FROM Trips t WHERE t.SafetyOperatorId = o.Id)",
            r => r.GetInt32(0));

        var actual = _service.GetOperatorsWithoutTrips().Select(o => o.Id).ToList();

        AssertSameSet(expected, actual);
    }

    [Fact(DisplayName = "GetTripsWithoutInterventions returns the departed trips of a day without interventions, ordered by planned departure.")]
    public void GetTripsWithoutInterventionsTest()
    {
        foreach (var date in new[] { new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 6), new DateOnly(2026, 10, 9) })
        {
            var expected = QueryDatabase($@"
                SELECT t.Id FROM Trips t
                WHERE date(t.PlannedDeparture) = '{date:yyyy-MM-dd}'
                  AND t.ActualDeparture IS NOT NULL
                  AND NOT EXISTS (SELECT 1 FROM Interventions i WHERE i.TripId = t.Id)
                ORDER BY t.PlannedDeparture",
                r => r.GetInt32(0));

            var actual = _service.GetTripsWithoutInterventions(date).Select(t => t.Id).ToList();

            AssertSameSequence(expected, actual, $"GetTripsWithoutInterventions({date:yyyy-MM-dd})");
        }
    }

    [Fact(DisplayName = "GetVehiclesWithInterventionReason returns the vehicles with an intervention of that reason, ordered by license plate.")]
    public void GetVehiclesWithInterventionReasonTest()
    {
        foreach (var reason in new[] { InterventionReason.Weather, InterventionReason.SensorFailure, InterventionReason.Obstacle })
        {
            var expected = QueryDatabase($@"
                SELECT v.Id FROM Vehicles v
                WHERE EXISTS (
                    SELECT 1 FROM Trips t JOIN Interventions i ON i.TripId = t.Id
                    WHERE t.VehicleId = v.Id AND i.Reason = '{reason}')
                ORDER BY v.LicensePlate",
                r => r.GetInt32(0));

            var actual = _service.GetVehiclesWithInterventionReason(reason).Select(v => v.Id).ToList();

            AssertSameSequence(expected, actual, $"GetVehiclesWithInterventionReason({reason})");
        }
    }

    [Fact(DisplayName = "IsLineOperatedAutonomously is true if no trip of the line that is not cancelled used a conventional bus.")]
    public void IsLineOperatedAutonomouslyTest()
    {
        foreach (var lineNumber in new[] { "A1", "A3", "N1" })
        {
            var expected = QueryDatabase($@"
                SELECT NOT EXISTS (
                    SELECT 1 FROM Trips t
                    JOIN Lines l ON l.Id = t.LineId
                    JOIN Vehicles v ON v.Id = t.VehicleId
                    WHERE l.Number = '{lineNumber}' AND t.IsCancelled = 0 AND v.IsAutonomous = 0)",
                r => r.GetBoolean(0)).Single();

            var actual = _service.IsLineOperatedAutonomously(lineNumber);

            Assert.True(expected == actual, $"IsLineOperatedAutonomously(\"{lineNumber}\"): expected {expected}, got {actual}.");
        }
    }

    [Fact(DisplayName = "GetStopsOfLine returns the stops of a line, ordered by position.")]
    public void GetStopsOfLineTest()
    {
        foreach (var lineNumber in new[] { "A3", "N1", "X9" })
        {
            var expected = QueryDatabase($@"
                SELECT ls.Position, s.Name, s.Municipality, ls.MinutesFromStart
                FROM LineStops ls
                JOIN Lines l ON l.Id = ls.LineId
                JOIN Stops s ON s.Id = ls.StopId
                WHERE l.Number = '{lineNumber}'
                ORDER BY ls.Position",
                r => new LineStopInfo(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetInt32(3)));

            var actual = _service.GetStopsOfLine(lineNumber);

            AssertSameSequence(expected, actual, $"GetStopsOfLine(\"{lineNumber}\")");
        }
    }

    [Fact(DisplayName = "GetTripSummaries returns all trips of a day with operator name and number of interventions.")]
    public void GetTripSummariesTest()
    {
        foreach (var date in new[] { new DateOnly(2026, 10, 6), new DateOnly(2026, 10, 7), new DateOnly(2026, 10, 10) })
        {
            var expected = QueryDatabase($@"
                SELECT t.Id, l.Number, v.LicensePlate,
                       CASE WHEN o.Id IS NULL THEN NULL ELSE o.FirstName || ' ' || o.LastName END,
                       t.PlannedDeparture, t.IsCancelled,
                       (SELECT COUNT(*) FROM Interventions i WHERE i.TripId = t.Id)
                FROM Trips t
                JOIN Lines l ON l.Id = t.LineId
                JOIN Vehicles v ON v.Id = t.VehicleId
                LEFT JOIN SafetyOperators o ON o.Id = t.SafetyOperatorId
                WHERE date(t.PlannedDeparture) = '{date:yyyy-MM-dd}'
                ORDER BY t.PlannedDeparture",
                r => new TripSummary(
                    r.GetInt32(0), r.GetString(1), r.GetString(2), r.IsDBNull(3) ? null : r.GetString(3),
                    r.GetDateTime(4), r.GetBoolean(5), r.GetInt32(6)));

            var actual = _service.GetTripSummaries(date);

            AssertSameSequence(expected, actual, $"GetTripSummaries({date:yyyy-MM-dd})");
        }
    }

    [Fact(DisplayName = "GetNextDeparture returns the next trip of the line that is not cancelled, or null.")]
    public void GetNextDepartureTest()
    {
        foreach (var (lineNumber, after) in new[]
        {
            ("A1", "2026-10-05 07:00:00"), ("A2", "2026-10-06 07:00:00"),
            ("A3", "2026-10-08 07:00:00"), ("N1", "2026-10-05 00:00:00")
        })
        {
            var expected = QueryDatabase($@"
                SELECT t.Id FROM Trips t JOIN Lines l ON l.Id = t.LineId
                WHERE l.Number = '{lineNumber}' AND t.IsCancelled = 0 AND t.PlannedDeparture > '{after}'
                ORDER BY t.PlannedDeparture
                LIMIT 1",
                r => (int?)r.GetInt32(0)).SingleOrDefault();

            var actual = _service.GetNextDeparture(lineNumber, DateTime.Parse(after));

            Assert.True(expected == actual?.Id,
                $"GetNextDeparture(\"{lineNumber}\", {after}): expected trip {expected?.ToString() ?? "null"}, got {actual?.Id.ToString() ?? "null"}.");
        }
    }

    [Fact(DisplayName = "CountPassengers sums up the passengers of a line in a period (both days inclusive).")]
    public void CountPassengersTest()
    {
        foreach (var (lineNumber, from, to) in new[]
        {
            ("A1", new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 6)),
            ("A2", new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 9)),
            ("N1", new DateOnly(2026, 10, 5), new DateOnly(2026, 10, 9))
        })
        {
            var expected = QueryDatabase($@"
                SELECT COALESCE(SUM(t.PassengerCount), 0) FROM Trips t JOIN Lines l ON l.Id = t.LineId
                WHERE l.Number = '{lineNumber}'
                  AND date(t.PlannedDeparture) BETWEEN '{from:yyyy-MM-dd}' AND '{to:yyyy-MM-dd}'",
                r => r.GetInt32(0)).Single();

            var actual = _service.CountPassengers(lineNumber, from, to);

            Assert.True(expected == actual, $"CountPassengers(\"{lineNumber}\", {from:yyyy-MM-dd}, {to:yyyy-MM-dd}): expected {expected}, got {actual}.");
        }
    }

    [Fact(DisplayName = "GetDelayedTrips returns the departed trips with at least the given delay, ordered by delay descending.")]
    public void GetDelayedTripsTest()
    {
        foreach (var minDelayMinutes in new[] { 5, 1, 60 })
        {
            var expected = QueryDatabase($@"
                SELECT Id, Number, PlannedDeparture, Delay FROM (
                    SELECT t.Id, l.Number, t.PlannedDeparture,
                           CAST(ROUND((julianday(t.ActualDeparture) - julianday(t.PlannedDeparture)) * 1440) AS INTEGER) AS Delay
                    FROM Trips t JOIN Lines l ON l.Id = t.LineId
                    WHERE t.ActualDeparture IS NOT NULL)
                WHERE Delay >= {minDelayMinutes}
                ORDER BY Delay DESC, Id",
                r => new DelayedTrip(r.GetInt32(0), r.GetString(1), r.GetDateTime(2), r.GetInt32(3)));

            var actual = _service.GetDelayedTrips(minDelayMinutes);

            AssertSameSequence(expected, actual, $"GetDelayedTrips({minDelayMinutes})");
        }
    }

    private static void AssertSameSequence<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual, string call)
    {
        Assert.True(expected.Count == actual.Count,
            $"{call}: expected {expected.Count} rows, got {actual.Count}.");
        for (int i = 0; i < expected.Count; i++)
            Assert.True(Equals(expected[i], actual[i]),
                $"{call}, row {i}: expected {expected[i]}, got {actual[i]}. Check the filter and the order.");
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
