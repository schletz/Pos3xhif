using ConsoleTables;
using Eventmanager.Infrastructure;
using Eventmanager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Diagnostics;

public class Program
{
    private static void Main(string[] args)
    {
        using var db = GetSeededDbContext();
        var eventService = new EventService(db);

        ConsoleTable
            .From(
                eventService.GetShowsInDateRange(new DateTime(2024, 2, 1), new DateTime(2024, 4, 1)))
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write();

        ConsoleTable
            .From(
                eventService.GetEventsWithShowCount())
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write();

        Console.WriteLine(Serialize(eventService.GetEventsWithShows(1)));

        eventService.CreateReservation(1, 1, 1, new DateTime(2024, 2, 1));
    }


    private static EventContext GetSeededDbContext()
    {
        var options = new DbContextOptionsBuilder()
            .UseSqlite("DataSource=../../../../events.db")
            .LogTo(
                message=>Debug.WriteLine(message), 
                new[] { DbLoggerCategory.Database.Command.Name }, 
                Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;

        var db = new EventContext(options);
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.Seed();
        return db;
    }

    private static string Serialize<T>(T data)
    {
        var options = new System.Text.Json.JsonSerializerOptions()
        {
            PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        return System.Text.Json.JsonSerializer.Serialize(data, options);

    }
}