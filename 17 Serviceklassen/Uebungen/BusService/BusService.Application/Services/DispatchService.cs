using System;
using System.Collections.Generic;
using System.Linq;
using BusService.Application.Dtos;
using BusService.Application.Infrastructure;
using BusService.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace BusService.Application.Services;

/// <summary>
/// Business logic of the dispatch center: reads lines and trips, plans trips and records
/// departures and interventions. All rules are described in README.md.
/// </summary>
public class DispatchService
{
    private readonly BusContext _db;

    public DispatchService(BusContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns every line with the number of its stops and of its trips that are not cancelled,
    /// ordered by line number.
    /// </summary>
    public List<LineDto> GetLines()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a line with its stops (ordered by position) or null if the line does not exist.
    /// </summary>
    public LineDetailDto? GetLineDetail(string lineNumber)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns a trip with its interventions (ordered by time) or null if the trip does not exist.
    /// </summary>
    public TripDetailDto? GetTripDetail(int tripId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Plans a new trip and returns it.
    /// </summary>
    /// <exception cref="DispatchServiceException">A business rule is violated.</exception>
    public Trip ScheduleTrip(int lineId, int vehicleId, int? safetyOperatorId, DateTime plannedDeparture)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stores the actual departure of a trip.
    /// </summary>
    /// <exception cref="DispatchServiceException">A business rule is violated.</exception>
    public void RecordDeparture(int tripId, DateTime actualDeparture)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Stores an intervention of the safety operator and returns it.
    /// stopId is null if the intervention happened between two stops.
    /// </summary>
    /// <exception cref="DispatchServiceException">A business rule is violated.</exception>
    public Intervention RecordIntervention(int tripId, int? stopId, DateTime time, InterventionReason reason, int durationSeconds)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Cancels a trip. The trip is not deleted, IsCancelled is set to true.
    /// </summary>
    /// <exception cref="DispatchServiceException">A business rule is violated.</exception>
    public void CancelTrip(int tripId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }
}
