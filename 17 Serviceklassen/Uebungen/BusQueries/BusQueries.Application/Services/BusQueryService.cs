using System;
using System.Collections.Generic;
using System.Linq;
using BusQueries.Application.Infrastructure;
using BusQueries.Application.Model;
using BusQueries.Application.Results;
using Microsoft.EntityFrameworkCore;

namespace BusQueries.Application.Services;

/// <summary>
/// Read-only queries for the dispatch center of the autonomous bus service.
/// </summary>
public class BusQueryService
{
    private readonly BusContext _db;

    public BusQueryService(BusContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all stops in the given municipality, ordered by name.
    /// </summary>
    public List<Stop> GetStopsInMunicipality(string municipality)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the license plates of all autonomous (autonomous = true) or conventional
    /// (autonomous = false) vehicles, ordered alphabetically.
    /// </summary>
    public List<string> GetLicensePlates(bool autonomous)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all safety operators who have no trip at all.
    /// </summary>
    public List<SafetyOperator> GetOperatorsWithoutTrips()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the departed trips of the given day without any intervention,
    /// ordered by the planned departure.
    /// </summary>
    public List<Trip> GetTripsWithoutInterventions(DateOnly date)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all vehicles that had at least one intervention with the given reason,
    /// ordered by license plate.
    /// </summary>
    public List<Vehicle> GetVehiclesWithInterventionReason(InterventionReason reason)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns true if every trip of the line that is not cancelled used an autonomous vehicle.
    /// </summary>
    public bool IsLineOperatedAutonomously(string lineNumber)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the stops of a line, ordered by position. Unknown lines return an empty list.
    /// </summary>
    public List<LineStopInfo> GetStopsOfLine(string lineNumber)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a summary of every trip of the given day, ordered by the planned departure.
    /// </summary>
    public List<TripSummary> GetTripSummaries(DateOnly date)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the first trip of the line that is not cancelled and departs after the given
    /// point in time, or null if there is none.
    /// </summary>
    public Trip? GetNextDeparture(string lineNumber, DateTime after)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the number of passengers of a line from the day "from" to the day "to"
    /// (both inclusive). Trips without passenger count are counted as 0.
    /// </summary>
    public int CountPassengers(string lineNumber, DateOnly from, DateOnly to)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all departed trips with a delay of at least minDelayMinutes, ordered by the
    /// delay (descending) and then by trip id.
    /// </summary>
    public List<DelayedTrip> GetDelayedTrips(int minDelayMinutes)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }
}
