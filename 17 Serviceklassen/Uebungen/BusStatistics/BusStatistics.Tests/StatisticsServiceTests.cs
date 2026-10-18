// *************************************************************************************************
// Tests for StatisticsService. DO NOT CHANGE THIS FILE.
// The expected values are calculated with SQL only, so the LINQ solution cannot be read from
// this file.
// *************************************************************************************************
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using BusStatistics.Application.Dtos;
using BusStatistics.Application.Infrastructure;
using BusStatistics.Application.Model;
using BusStatistics.Application.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BusStatistics.Tests;

public class StatisticsServiceTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly BusContext _db;
    private readonly StatisticsService _service;

    public StatisticsServiceTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder().UseSqlite(_connection).Options;
        _db = new BusContext(options);
        _db.Database.EnsureCreated();
        _db.Seed();
        _service = new StatisticsService(_db);
    }

    [Fact(DisplayName = "GetInterventionsByReason groups the interventions by reason.")]
    public void GetInterventionsByReasonTest()
    {
        var expected = QueryDatabase(@"
            SELECT Reason, COUNT(*), SUM(DurationSeconds) FROM Interventions
            GROUP BY Reason ORDER BY COUNT(*) DESC, SUM(DurationSeconds) DESC",
            r => new ReasonStatistics(Enum.Parse<InterventionReason>(r.GetString(0)), r.GetInt32(1), r.GetInt32(2)));

        var actual = _service.GetInterventionsByReason();

        AssertSameSequence(expected, actual, "GetInterventionsByReason()");
    }

    [Fact(DisplayName = "GetPassengerStatisticsPerLine groups the counted trips by line.")]
    public void GetPassengerStatisticsPerLineTest()
    {
        var expected = QueryDatabase(@"
            SELECT l.Number, COUNT(*), SUM(t.PassengerCount), AVG(t.PassengerCount)
            FROM Trips t JOIN Lines l ON l.Id = t.LineId
            WHERE t.PassengerCount IS NOT NULL
            GROUP BY l.Number ORDER BY l.Number",
            r => new LinePassengerStatistics(r.GetString(0), r.GetInt32(1), r.GetInt32(2), r.GetDouble(3)));

        var actual = _service.GetPassengerStatisticsPerLine();

        Assert.True(expected.Count == actual.Count, $"Expected {expected.Count} lines, got {actual.Count}.");
        for (int i = 0; i < expected.Count; i++)
        {
            var e = expected[i];
            var a = actual[i];
            Assert.True(e.LineNumber == a.LineNumber && e.TripCount == a.TripCount && e.TotalPassengers == a.TotalPassengers
                    && Math.Abs(e.AveragePassengers - a.AveragePassengers) < 0.001,
                $"Row {i}: expected {e}, got {a}.");
        }
    }

    [Fact(DisplayName = "GetInterventionsPerDay groups the interventions by day.")]
    public void GetInterventionsPerDayTest()
    {
        var expected = QueryDatabase(@"
            SELECT date(Time), COUNT(*), SUM(DurationSeconds) FROM Interventions
            GROUP BY date(Time) ORDER BY date(Time)",
            r => new DailyInterventions(DateOnly.Parse(r.GetString(0)), r.GetInt32(1), r.GetInt32(2)));

        var actual = _service.GetInterventionsPerDay();

        AssertSameSequence(expected, actual, "GetInterventionsPerDay()");
    }

    [Fact(DisplayName = "GetStopNamesServedByVehicle returns the distinct stop names of the vehicle's lines.")]
    public void GetStopNamesServedByVehicleTest()
    {
        foreach (var licensePlate in new[] { "DO-AUTO1", "DO-AUTO3", "DO-BUS40", "DO-AUTO4" })
        {
            var expected = QueryDatabase($@"
                SELECT DISTINCT s.Name
                FROM Trips t
                JOIN Vehicles v ON v.Id = t.VehicleId
                JOIN LineStops ls ON ls.LineId = t.LineId
                JOIN Stops s ON s.Id = ls.StopId
                WHERE v.LicensePlate = '{licensePlate}' AND t.IsCancelled = 0
                ORDER BY s.Name",
                r => r.GetString(0));

            var actual = _service.GetStopNamesServedByVehicle(licensePlate);

            AssertSameSequence(expected, actual, $"GetStopNamesServedByVehicle(\"{licensePlate}\")");
        }
    }

    [Fact(DisplayName = "GetInterventionsOfOperator returns all interventions of the operator's trips.")]
    public void GetInterventionsOfOperatorTest()
    {
        foreach (var operatorId in new[] { 1, 3, 4 })
        {
            var expected = QueryDatabase($@"
                SELECT i.Id FROM Interventions i JOIN Trips t ON t.Id = i.TripId
                WHERE t.SafetyOperatorId = {operatorId} ORDER BY i.Time",
                r => r.GetInt32(0));

            var actual = _service.GetInterventionsOfOperator(operatorId).Select(i => i.Id).ToList();

            AssertSameSequence(expected, actual, $"GetInterventionsOfOperator({operatorId})");
        }
    }

    [Fact(DisplayName = "GetInterventionsByWeather joins the interventions with the weather readings by date.")]
    public void GetInterventionsByWeatherTest()
    {
        var expected = QueryDatabase(@"
            SELECT w.Condition, COUNT(*), SUM(i.DurationSeconds)
            FROM Interventions i JOIN WeatherReadings w ON w.Date = date(i.Time)
            GROUP BY w.Condition ORDER BY COUNT(*) DESC, w.Condition",
            r => new WeatherStatistics(r.GetString(0), r.GetInt32(1), r.GetInt32(2)));

        var actual = _service.GetInterventionsByWeather();

        AssertSameSequence(expected, actual, "GetInterventionsByWeather()");
    }

    [Fact(DisplayName = "GetInterventionDaysWithWeather returns every day with interventions, also without weather reading.")]
    public void GetInterventionDaysWithWeatherTest()
    {
        var expected = QueryDatabase(@"
            SELECT d.Day, d.Count, w.Condition
            FROM (SELECT date(Time) AS Day, COUNT(*) AS Count FROM Interventions GROUP BY date(Time)) d
            LEFT JOIN WeatherReadings w ON w.Date = d.Day
            ORDER BY d.Day",
            r => new DayWeather(DateOnly.Parse(r.GetString(0)), r.GetInt32(1), r.IsDBNull(2) ? null : r.GetString(2)));

        var actual = _service.GetInterventionDaysWithWeather();

        AssertSameSequence(expected, actual, "GetInterventionDaysWithWeather()");
    }

    [Fact(DisplayName = "GetRouteDescription joins the stop names of a line with \" -> \".")]
    public void GetRouteDescriptionTest()
    {
        foreach (var lineNumber in new[] { "A1", "A3", "N1", "X9" })
        {
            var expected = QueryDatabase($@"
                SELECT COALESCE(group_concat(s.Name, ' -> ' ORDER BY ls.Position), '')
                FROM LineStops ls JOIN Lines l ON l.Id = ls.LineId JOIN Stops s ON s.Id = ls.StopId
                WHERE l.Number = '{lineNumber}'",
                r => r.GetString(0)).Single();

            var actual = _service.GetRouteDescription(lineNumber);

            Assert.True(expected == actual, $"GetRouteDescription(\"{lineNumber}\"): expected \"{expected}\", got \"{actual}\".");
        }
    }

    [Fact(DisplayName = "GetTotalInterventionTime adds up the durations of a trip as TimeSpan.")]
    public void GetTotalInterventionTimeTest()
    {
        foreach (var tripId in new[] { 2, 11, 4, 999 })
        {
            var seconds = QueryDatabase($"SELECT COALESCE(SUM(DurationSeconds), 0) FROM Interventions WHERE TripId = {tripId}",
                r => r.GetInt32(0)).Single();
            var expected = TimeSpan.FromSeconds(seconds);

            var actual = _service.GetTotalInterventionTime(tripId);

            Assert.True(expected == actual, $"GetTotalInterventionTime({tripId}): expected {expected}, got {actual}.");
        }
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
