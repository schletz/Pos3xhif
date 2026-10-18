// *************************************************************************************************
// Tests for DispatchService. DO NOT CHANGE THIS FILE.
// The expected values are calculated with SQL only, so the LINQ solution cannot be read from
// this file. The DTO records are written by you, so the tests read their properties with
// reflection. The names of the properties are case-insensitive.
// *************************************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using BusService.Application.Infrastructure;
using BusService.Application.Model;
using BusService.Application.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BusService.Tests;

public class DispatchServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly BusContext _db;
    private readonly DispatchService _service;

    public DispatchServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new BusContext(options);
        _db.Database.EnsureCreated();
        _db.Seed();
        _service = new DispatchService(_db);
    }

    // ===== Reading methods with DTOs ==============================================================

    [Fact(DisplayName = "GetLines returns every line with the number of stops and of trips that are not cancelled.")]
    public void GetLinesTest()
    {
        var expected = QueryDatabase(@"
            SELECT l.Id, l.Number, l.Name,
                   (SELECT COUNT(*) FROM LineStops ls WHERE ls.LineId = l.Id),
                   (SELECT COUNT(*) FROM Trips t WHERE t.LineId = l.Id AND t.IsCancelled = 0)
            FROM Lines l ORDER BY l.Number",
            r => $"{r.GetInt32(0)}|{r.GetString(1)}|{r.GetString(2)}|{r.GetInt32(3)}|{r.GetInt32(4)}");

        var actual = _service.GetLines()
            .Select(d => $"{Get<int>(d, "Id")}|{Get<string>(d, "Number")}|{Get<string>(d, "Name")}|{Get<int>(d, "StopCount")}|{Get<int>(d, "TripCount")}")
            .ToList();

        AssertSameSequence(expected, actual, "GetLines()");
    }

    [Fact(DisplayName = "GetLineDetail returns a line with its stops ordered by position, or null.")]
    public void GetLineDetailTest()
    {
        foreach (var lineNumber in new[] { "A2", "A3", "N1" })
        {
            var line = QueryDatabase($"SELECT Number, Name FROM Lines WHERE Number = '{lineNumber}'",
                r => $"{r.GetString(0)}|{r.GetString(1)}").Single();
            var expectedStops = QueryDatabase($@"
                SELECT ls.Position, s.Name, ls.MinutesFromStart
                FROM LineStops ls JOIN Lines l ON l.Id = ls.LineId JOIN Stops s ON s.Id = ls.StopId
                WHERE l.Number = '{lineNumber}' ORDER BY ls.Position",
                r => $"{r.GetInt32(0)}|{r.GetString(1)}|{r.GetInt32(2)}");

            var actual = _service.GetLineDetail(lineNumber);

            Assert.True(actual is not null, $"GetLineDetail(\"{lineNumber}\") returned null.");
            Assert.Equal(line, $"{Get<string>(actual, "Number")}|{Get<string>(actual, "Name")}");
            var actualStops = GetList(actual, "Stops")
                .Select(s => $"{Get<int>(s, "Position")}|{Get<string>(s, "StopName")}|{Get<int>(s, "MinutesFromStart")}")
                .ToList();
            AssertSameSequence(expectedStops, actualStops, $"GetLineDetail(\"{lineNumber}\").Stops");
        }
        Assert.True(_service.GetLineDetail("X9") is null, "GetLineDetail(\"X9\") must return null.");
    }

    [Fact(DisplayName = "GetTripDetail returns a trip with its interventions ordered by time, or null.")]
    public void GetTripDetailTest()
    {
        foreach (var tripId in new[] { 2, 4, 7, 11 })
        {
            var trip = QueryDatabase($@"
                SELECT t.Id, l.Number, v.LicensePlate,
                       CASE WHEN o.Id IS NULL THEN NULL ELSE o.FirstName || ' ' || o.LastName END,
                       t.PlannedDeparture, t.ActualDeparture, t.IsCancelled
                FROM Trips t
                JOIN Lines l ON l.Id = t.LineId
                JOIN Vehicles v ON v.Id = t.VehicleId
                LEFT JOIN SafetyOperators o ON o.Id = t.SafetyOperatorId
                WHERE t.Id = {tripId}",
                r => $"{r.GetInt32(0)}|{r.GetString(1)}|{r.GetString(2)}|{(r.IsDBNull(3) ? "null" : r.GetString(3))}|" +
                     $"{r.GetDateTime(4):s}|{(r.IsDBNull(5) ? "null" : r.GetDateTime(5).ToString("s"))}|{r.GetBoolean(6)}").Single();
            var expectedInterventions = QueryDatabase($@"
                SELECT i.Time, s.Name, i.Reason, i.DurationSeconds
                FROM Interventions i LEFT JOIN Stops s ON s.Id = i.StopId
                WHERE i.TripId = {tripId} ORDER BY i.Time",
                r => $"{r.GetDateTime(0):s}|{(r.IsDBNull(1) ? "null" : r.GetString(1))}|{r.GetString(2)}|{r.GetInt32(3)}");

            var actual = _service.GetTripDetail(tripId);

            Assert.True(actual is not null, $"GetTripDetail({tripId}) returned null.");
            Assert.Equal(trip,
                $"{Get<int>(actual, "TripId")}|{Get<string>(actual, "LineNumber")}|{Get<string>(actual, "LicensePlate")}|" +
                $"{Get<string?>(actual, "OperatorName") ?? "null"}|{Get<DateTime>(actual, "PlannedDeparture"):s}|" +
                $"{Get<DateTime?>(actual, "ActualDeparture")?.ToString("s") ?? "null"}|{Get<bool>(actual, "IsCancelled")}");
            var actualInterventions = GetList(actual, "Interventions")
                .Select(i => $"{Get<DateTime>(i, "Time"):s}|{Get<string?>(i, "StopName") ?? "null"}|" +
                             $"{Get<InterventionReason>(i, "Reason")}|{Get<int>(i, "DurationSeconds")}")
                .ToList();
            AssertSameSequence(expectedInterventions, actualInterventions, $"GetTripDetail({tripId}).Interventions");
        }
        Assert.True(_service.GetTripDetail(999) is null, "GetTripDetail(999) must return null.");
    }

    // ===== ScheduleTrip ===========================================================================

    [Fact(DisplayName = "ScheduleTrip creates a trip for an autonomous vehicle and for a conventional bus.")]
    public void ScheduleTripCreatesTripTest()
    {
        var planned = new DateTime(2026, 10, 9, 9, 0, 0);
        var trip = _service.ScheduleTrip(LineId("A1"), VehicleId("DO-AUTO1"), OperatorId("Moser"), planned);
        AssertStoredTrip(trip.Id, LineId("A1"), VehicleId("DO-AUTO1"), OperatorId("Moser"), planned);

        var busTrip = _service.ScheduleTrip(LineId("A3"), VehicleId("DO-BUS40"), null, planned);
        AssertStoredTrip(busTrip.Id, LineId("A3"), VehicleId("DO-BUS40"), null, planned);
    }

    [Fact(DisplayName = "ScheduleTrip checks that line, vehicle and safety operator exist.")]
    public void ScheduleTripChecksIdsTest()
    {
        var planned = new DateTime(2026, 10, 9, 9, 0, 0);
        AssertThrows("Line not found.", () => _service.ScheduleTrip(999, VehicleId("DO-AUTO1"), OperatorId("Moser"), planned));
        AssertThrows("Vehicle not found.", () => _service.ScheduleTrip(LineId("A1"), 999, OperatorId("Moser"), planned));
        AssertThrows("Safety operator not found.", () => _service.ScheduleTrip(LineId("A1"), VehicleId("DO-AUTO1"), 999, planned));
        AssertTripCount(16);
    }

    [Fact(DisplayName = "ScheduleTrip checks the safety operator rules for autonomous and conventional vehicles.")]
    public void ScheduleTripChecksOperatorRuleTest()
    {
        var planned = new DateTime(2026, 10, 9, 9, 0, 0);
        AssertThrows("An autonomous vehicle needs a safety operator.",
            () => _service.ScheduleTrip(LineId("A1"), VehicleId("DO-AUTO1"), null, planned));
        AssertThrows("A conventional bus has no safety operator.",
            () => _service.ScheduleTrip(LineId("A3"), VehicleId("DO-BUS40"), OperatorId("Moser"), planned));
        AssertTripCount(16);
    }

    [Fact(DisplayName = "ScheduleTrip checks the certification of the safety operator (the last day is valid).")]
    public void ScheduleTripChecksCertificationTest()
    {
        // Emre Yilmaz is certified until 7 October 2026.
        AssertThrows("The certification of the safety operator has expired.",
            () => _service.ScheduleTrip(LineId("A2"), VehicleId("DO-AUTO4"), OperatorId("Yilmaz"), new DateTime(2026, 10, 8, 6, 0, 0)));
        var planned = new DateTime(2026, 10, 7, 21, 0, 0);
        var trip = _service.ScheduleTrip(LineId("A2"), VehicleId("DO-AUTO4"), OperatorId("Yilmaz"), planned);
        AssertStoredTrip(trip.Id, LineId("A2"), VehicleId("DO-AUTO4"), OperatorId("Yilmaz"), planned);
    }

    [Fact(DisplayName = "ScheduleTrip checks that the vehicle has no other trip at the same time (cancelled trips do not count).")]
    public void ScheduleTripChecksVehicleAvailabilityTest()
    {
        // Trip 1: DO-AUTO1 on line A1 (9 minutes) on Monday from 07:00 to 07:09.
        // Line A2 takes 12 minutes, so 06:50 to 07:02 overlaps.
        AssertThrows("The vehicle is not available.",
            () => _service.ScheduleTrip(LineId("A2"), VehicleId("DO-AUTO1"), OperatorId("Moser"), new DateTime(2026, 10, 5, 6, 50, 0)));
        // 07:05 to 07:14 starts during trip 1.
        AssertThrows("The vehicle is not available.",
            () => _service.ScheduleTrip(LineId("A1"), VehicleId("DO-AUTO1"), OperatorId("Moser"), new DateTime(2026, 10, 5, 7, 5, 0)));
        AssertTripCount(16);

        // 07:09 to 07:18 starts exactly when trip 1 ends and ends before trip 2 (07:30).
        var planned = new DateTime(2026, 10, 5, 7, 9, 0);
        var trip = _service.ScheduleTrip(LineId("A1"), VehicleId("DO-AUTO1"), OperatorId("Moser"), planned);
        AssertStoredTrip(trip.Id, LineId("A1"), VehicleId("DO-AUTO1"), OperatorId("Moser"), planned);

        // Trip 7 (DO-AUTO2, Tuesday 07:15) is cancelled, the vehicle is free.
        planned = new DateTime(2026, 10, 6, 7, 15, 0);
        trip = _service.ScheduleTrip(LineId("A2"), VehicleId("DO-AUTO2"), OperatorId("Yilmaz"), planned);
        AssertStoredTrip(trip.Id, LineId("A2"), VehicleId("DO-AUTO2"), OperatorId("Yilmaz"), planned);
    }

    // ===== RecordDeparture, RecordIntervention, CancelTrip ========================================

    [Fact(DisplayName = "RecordDeparture stores the departure and checks the rules.")]
    public void RecordDepartureTest()
    {
        AssertThrows("Trip not found.", () => _service.RecordDeparture(999, new DateTime(2026, 10, 9, 7, 2, 0)));
        AssertThrows("The trip is cancelled.", () => _service.RecordDeparture(7, new DateTime(2026, 10, 6, 7, 16, 0)));
        AssertThrows("The trip has already departed.", () => _service.RecordDeparture(1, new DateTime(2026, 10, 5, 7, 2, 0)));

        var departure = new DateTime(2026, 10, 9, 7, 3, 0);
        _service.RecordDeparture(15, departure);

        _db.ChangeTracker.Clear();
        var stored = QueryDatabase("SELECT ActualDeparture FROM Trips WHERE Id = 15", r => r.IsDBNull(0) ? (DateTime?)null : r.GetDateTime(0)).Single();
        Assert.True(stored == departure, $"ActualDeparture of trip 15: expected {departure:s}, got {stored?.ToString("s") ?? "null"}.");
        var unchanged = QueryDatabase("SELECT ActualDeparture FROM Trips WHERE Id = 16", r => r.IsDBNull(0)).Single();
        Assert.True(unchanged, "RecordDeparture must not change other trips.");
    }

    [Fact(DisplayName = "RecordIntervention stores an intervention (with or without stop) and checks the rules.")]
    public void RecordInterventionTest()
    {
        var time = new DateTime(2026, 10, 5, 7, 7, 0);
        AssertThrows("Trip not found.", () => _service.RecordIntervention(999, null, time, InterventionReason.Obstacle, 30));
        AssertThrows("The trip has not departed yet.", () => _service.RecordIntervention(15, null, time, InterventionReason.Obstacle, 30));
        AssertThrows("The trip has not departed yet.", () => _service.RecordIntervention(7, null, time, InterventionReason.Obstacle, 30));
        // Rheinstraße (id 5) is not a stop of line A1.
        AssertThrows("The stop is not served by this line.", () => _service.RecordIntervention(1, 5, time, InterventionReason.Obstacle, 30));
        AssertThrows("The duration must be positive.", () => _service.RecordIntervention(1, 3, time, InterventionReason.Obstacle, 0));
        Assert.Equal(12, QueryDatabase("SELECT COUNT(*) FROM Interventions", r => r.GetInt32(0)).Single());

        // FH Campus (id 3) is a stop of line A1.
        var atStop = _service.RecordIntervention(1, 3, time, InterventionReason.Passenger, 75);
        var betweenStops = _service.RecordIntervention(1, null, time.AddMinutes(1), InterventionReason.Traffic, 20);

        _db.ChangeTracker.Clear();
        var stored = QueryDatabase($@"
            SELECT Id, TripId, StopId, Time, Reason, DurationSeconds FROM Interventions
            WHERE Id IN ({atStop.Id}, {betweenStops.Id}) ORDER BY Time",
            r => $"{r.GetInt32(1)}|{(r.IsDBNull(2) ? "null" : r.GetInt32(2).ToString())}|{r.GetDateTime(3):s}|{r.GetString(4)}|{r.GetInt32(5)}");
        AssertSameSequence(
            new List<string> { $"1|3|{time:s}|Passenger|75", $"1|null|{time.AddMinutes(1):s}|Traffic|20" },
            stored, "Stored interventions");
    }

    [Fact(DisplayName = "CancelTrip sets IsCancelled and checks the rules.")]
    public void CancelTripTest()
    {
        AssertThrows("Trip not found.", () => _service.CancelTrip(999));
        AssertThrows("The trip is already cancelled.", () => _service.CancelTrip(7));
        AssertThrows("The trip has already departed.", () => _service.CancelTrip(1));

        _service.CancelTrip(15);

        _db.ChangeTracker.Clear();
        var cancelled = QueryDatabase("SELECT Id FROM Trips WHERE IsCancelled = 1 ORDER BY Id", r => r.GetInt32(0));
        AssertSameSequence(new List<int> { 7, 14, 15 }, cancelled, "Cancelled trips");
        Assert.Equal(16, QueryDatabase("SELECT COUNT(*) FROM Trips", r => r.GetInt32(0)).Single());
    }

    // ===== Helpers ================================================================================

    private int LineId(string number) => QueryDatabase($"SELECT Id FROM Lines WHERE Number = '{number}'", r => r.GetInt32(0)).Single();
    private int VehicleId(string plate) => QueryDatabase($"SELECT Id FROM Vehicles WHERE LicensePlate = '{plate}'", r => r.GetInt32(0)).Single();
    private int OperatorId(string lastName) => QueryDatabase($"SELECT Id FROM SafetyOperators WHERE LastName = '{lastName}'", r => r.GetInt32(0)).Single();

    private void AssertStoredTrip(int tripId, int lineId, int vehicleId, int? operatorId, DateTime planned)
    {
        Assert.True(tripId != 0, "The returned trip has no id. Did you call SaveChanges?");
        _db.ChangeTracker.Clear();
        var stored = QueryDatabase($@"
            SELECT LineId, VehicleId, SafetyOperatorId, PlannedDeparture, ActualDeparture, IsCancelled, PassengerCount
            FROM Trips WHERE Id = {tripId}",
            r => $"{r.GetInt32(0)}|{r.GetInt32(1)}|{(r.IsDBNull(2) ? "null" : r.GetInt32(2).ToString())}|{r.GetDateTime(3):s}|" +
                 $"{(r.IsDBNull(4) ? "null" : "set")}|{r.GetBoolean(5)}|{(r.IsDBNull(6) ? "null" : "set")}").SingleOrDefault();
        var expected = $"{lineId}|{vehicleId}|{operatorId?.ToString() ?? "null"}|{planned:s}|null|False|null";
        Assert.True(expected == stored, $"Stored trip {tripId}: expected {expected}, got {stored ?? "no row"}.");
    }

    private void AssertTripCount(int expected)
    {
        var count = QueryDatabase("SELECT COUNT(*) FROM Trips", r => r.GetInt32(0)).Single();
        Assert.True(expected == count, $"Expected {expected} trips in the database, found {count}. A rejected trip must not be stored.");
    }

    private static void AssertThrows(string message, Action action)
    {
        var e = Assert.Throws<DispatchServiceException>(action);
        Assert.Equal(message, e.Message);
    }

    /// <summary>
    /// Reads a property of a DTO with reflection. The name is case-insensitive.
    /// </summary>
    private static T Get<T>(object? dto, string propertyName)
    {
        Assert.True(dto is not null, $"Expected a DTO with the property {propertyName}, got null.");
        var property = dto.GetType().GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        Assert.True(property is not null, $"{dto.GetType().Name} has no property {propertyName}.");
        var value = property.GetValue(dto);
        Assert.True(value is T || (value is null && default(T) is null),
            $"{dto.GetType().Name}.{property.Name} must be of type {typeof(T).Name}.");
        return (T)value!;
    }

    /// <summary>
    /// Reads a list property of a DTO (e.g. List&lt;LineStopDto&gt;) as a list of objects.
    /// </summary>
    private static List<object> GetList(object? dto, string propertyName)
    {
        var list = Get<IEnumerable>(dto, propertyName);
        Assert.True(list is not null, $"{propertyName} must not be null.");
        return list.Cast<object>().ToList();
    }

    private static void AssertSameSequence<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual, string call)
    {
        Assert.True(expected.Count == actual.Count,
            $"{call}: expected {expected.Count} rows, got {actual.Count}.");
        for (int i = 0; i < expected.Count; i++)
            Assert.True(Equals(expected[i], actual[i]),
                $"{call}, row {i}: expected {expected[i]}, got {actual[i]}. Check the values and the order.");
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
