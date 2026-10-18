using System;
using System.Collections.Generic;
using BusQueries.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace BusQueries.Application.Infrastructure;

/// <summary>
/// DbContext of the autonomous bus service. <see cref="Seed"/> writes a small, hand-written
/// data set for one week (Monday 5 to Friday 9 October 2026). All data is fictional.
/// </summary>
public class BusContext : DbContext
{
    public DbSet<Line> Lines => Set<Line>();
    public DbSet<Stop> Stops => Set<Stop>();
    public DbSet<LineStop> LineStops => Set<LineStop>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<SafetyOperator> SafetyOperators => Set<SafetyOperator>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Intervention> Interventions => Set<Intervention>();

    public BusContext(DbContextOptions options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Line>().HasIndex(l => l.Number).IsUnique();
        modelBuilder.Entity<Vehicle>().HasIndex(v => v.LicensePlate).IsUnique();
        modelBuilder.Entity<LineStop>().HasIndex("LineId", nameof(LineStop.Position)).IsUnique();
        modelBuilder.Entity<Intervention>().Property(i => i.Reason).HasConversion<string>();

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // ON DELETE RESTRICT instead of ON DELETE CASCADE
            foreach (var key in entityType.GetForeignKeys())
                key.DeleteBehavior = DeleteBehavior.Restrict;
            foreach (var prop in entityType.GetDeclaredProperties())
            {
                // Default MaxLength of string Properties is 255.
                if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
            }
        }
    }

    /// <summary>
    /// Writes the sample data. The ids are set explicitly, so they are the same in every run.
    /// </summary>
    public void Seed()
    {
        var stops = new List<Stop>
        {
            new Stop("Bahnhof Dornbirn", "Dornbirn") { Id = 1 },
            new Stop("Marktplatz", "Dornbirn") { Id = 2 },
            new Stop("FH Campus", "Dornbirn") { Id = 3 },
            new Stop("Messepark", "Dornbirn") { Id = 4 },
            new Stop("Rheinstraße", "Lustenau") { Id = 5 },
            new Stop("Kirchplatz", "Lustenau") { Id = 6 },
            new Stop("Bahnhof Hohenems", "Hohenems") { Id = 7 },
            // Not served by any line.
            new Stop("Betriebshof", "Dornbirn") { Id = 8 },
        };
        Stops.AddRange(stops);

        var a1 = new Line("A1", "Bahnhof - FH Campus") { Id = 1 };
        var a2 = new Line("A2", "Messepark - Lustenau") { Id = 2 };
        var a3 = new Line("A3", "Dornbirn - Hohenems") { Id = 3 };
        // A night line without trips.
        var n1 = new Line("N1", "Nachtshuttle") { Id = 4 };
        Lines.AddRange(a1, a2, a3, n1);

        LineStops.AddRange(
            new LineStop(a1, stops[0], 1, 0) { Id = 1 },
            new LineStop(a1, stops[1], 2, 4) { Id = 2 },
            new LineStop(a1, stops[2], 3, 9) { Id = 3 },
            new LineStop(a2, stops[3], 1, 0) { Id = 4 },
            new LineStop(a2, stops[4], 2, 7) { Id = 5 },
            new LineStop(a2, stops[5], 3, 12) { Id = 6 },
            new LineStop(a3, stops[0], 1, 0) { Id = 7 },
            new LineStop(a3, stops[3], 2, 6) { Id = 8 },
            new LineStop(a3, stops[6], 3, 15) { Id = 9 },
            new LineStop(n1, stops[0], 1, 0) { Id = 10 },
            new LineStop(n1, stops[1], 2, 5) { Id = 11 });

        var auto1 = new Vehicle("DO-AUTO1", "CityShuttle S8", 15, true, new DateOnly(2026, 3, 2)) { Id = 1 };
        var auto2 = new Vehicle("DO-AUTO2", "CityShuttle S8", 15, true, new DateOnly(2026, 3, 2)) { Id = 2 };
        var auto3 = new Vehicle("DO-AUTO3", "MetroPod X12", 22, true, new DateOnly(2026, 9, 14)) { Id = 3 };
        var bus40 = new Vehicle("DO-BUS40", "Linienbus L40", 40, false, new DateOnly(2019, 5, 10)) { Id = 4 };
        // A new vehicle without trips.
        var auto4 = new Vehicle("DO-AUTO4", "MetroPod X12", 22, true, new DateOnly(2026, 10, 1)) { Id = 5 };
        Vehicles.AddRange(auto1, auto2, auto3, bus40, auto4);

        var moser = new SafetyOperator("Lena", "Moser", new DateOnly(2027, 6, 30)) { Id = 1 };
        // The certification expires during the week.
        var yilmaz = new SafetyOperator("Emre", "Yilmaz", new DateOnly(2026, 10, 7)) { Id = 2 };
        var berger = new SafetyOperator("Sophie", "Berger", new DateOnly(2027, 12, 31)) { Id = 3 };
        // An operator without trips.
        var fink = new SafetyOperator("Jonas", "Fink", new DateOnly(2027, 3, 31)) { Id = 4 };
        SafetyOperators.AddRange(moser, yilmaz, berger, fink);

        Trip Departed(int id, Line line, Vehicle vehicle, SafetyOperator? op, DateTime planned, int delayMinutes, int passengers) =>
            new Trip(line, vehicle, op, planned)
            {
                Id = id, ActualDeparture = planned.AddMinutes(delayMinutes), PassengerCount = passengers
            };
        Trip Cancelled(int id, Line line, Vehicle vehicle, SafetyOperator? op, DateTime planned) =>
            new Trip(line, vehicle, op, planned) { Id = id, IsCancelled = true };
        Trip Planned(int id, Line line, Vehicle vehicle, SafetyOperator? op, DateTime planned) =>
            new Trip(line, vehicle, op, planned) { Id = id };
        DateTime At(int day, int hour, int minute) => new DateTime(2026, 10, day, hour, minute, 0);

        var trips = new List<Trip>
        {
            // Monday, 5 October
            Departed(1, a1, auto1, moser, At(5, 7, 0), 1, 9),
            Departed(2, a1, auto1, moser, At(5, 7, 30), 6, 12),
            Departed(3, a2, auto2, yilmaz, At(5, 7, 15), 0, 5),
            Departed(4, a3, bus40, null, At(5, 8, 0), 3, 31),
            // Tuesday, 6 October
            Departed(5, a1, auto1, moser, At(6, 7, 0), 0, 11),
            Departed(6, a1, auto3, berger, At(6, 7, 30), 12, 14),
            Cancelled(7, a2, auto2, yilmaz, At(6, 7, 15)),
            Departed(8, a3, bus40, null, At(6, 8, 0), 1, 28),
            // Wednesday, 7 October
            Departed(9, a1, auto1, moser, At(7, 7, 0), 4, 10),
            Departed(10, a2, auto2, yilmaz, At(7, 7, 15), 2, 6),
            Departed(11, a3, auto3, berger, At(7, 8, 0), 9, 18),
            // Thursday, 8 October
            Departed(12, a1, auto1, moser, At(8, 7, 0), 0, 13),
            Departed(13, a2, auto3, berger, At(8, 7, 15), 5, 7),
            Cancelled(14, a3, bus40, null, At(8, 8, 0)),
            // Friday, 9 October: planned, not departed yet
            Planned(15, a1, auto1, moser, At(9, 7, 0)),
            Planned(16, a2, auto2, berger, At(9, 7, 15)),
        };
        // A trip without passenger count.
        trips[9].PassengerCount = null;
        Trips.AddRange(trips);

        Intervention Take(int id, int tripId, Stop? stop, DateTime time, InterventionReason reason, int seconds) =>
            new Intervention(trips[tripId - 1], stop, time, reason, seconds) { Id = id };

        Interventions.AddRange(
            Take(1, 1, stops[1], At(5, 7, 5), InterventionReason.Obstacle, 40),
            Take(2, 2, stops[1], At(5, 7, 40), InterventionReason.Passenger, 90),
            // Between two stops.
            Take(3, 2, null, At(5, 7, 43), InterventionReason.Traffic, 60),
            Take(4, 3, stops[4], At(5, 7, 22), InterventionReason.SensorFailure, 180),
            Take(5, 6, stops[0], At(6, 7, 42), InterventionReason.Weather, 300),
            Take(6, 6, stops[2], At(6, 7, 53), InterventionReason.Weather, 120),
            Take(7, 9, stops[1], At(7, 7, 8), InterventionReason.Obstacle, 30),
            Take(8, 10, stops[5], At(7, 7, 29), InterventionReason.Obstacle, 45),
            Take(9, 11, stops[3], At(7, 8, 15), InterventionReason.SensorFailure, 240),
            Take(10, 11, stops[6], At(7, 8, 24), InterventionReason.Passenger, 60),
            Take(11, 13, stops[4], At(8, 7, 27), InterventionReason.Weather, 150),
            Take(12, 13, stops[5], At(8, 7, 32), InterventionReason.Weather, 90));

        SaveChanges();
        ChangeTracker.Clear();
    }
}
