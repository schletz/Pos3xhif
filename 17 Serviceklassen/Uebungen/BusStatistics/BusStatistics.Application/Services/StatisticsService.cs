using System;
using System.Collections.Generic;
using System.Linq;
using BusStatistics.Application.Dtos;
using BusStatistics.Application.Infrastructure;
using BusStatistics.Application.Model;
using Microsoft.EntityFrameworkCore;

namespace BusStatistics.Application.Services;

/// <summary>
/// Statistics for the operator of the autonomous bus service: GroupBy, SelectMany, Join and
/// Aggregate. All requirements are described in README.md.
/// </summary>
public class StatisticsService
{
    private readonly BusContext _db;

    public StatisticsService(BusContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Number and total duration of the interventions per reason, ordered by number (descending)
    /// and then by total duration (descending). Reasons without interventions are not included.
    /// </summary>
    public List<ReasonStatistics> GetInterventionsByReason()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Number of counted trips, total and average passengers per line, ordered by line number.
    /// Only trips with a passenger count are considered.
    /// </summary>
    public List<LinePassengerStatistics> GetPassengerStatisticsPerLine()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Number and total duration of the interventions per day, ordered by date.
    /// </summary>
    public List<DailyInterventions> GetInterventionsPerDay()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Names of all stops on the lines the vehicle drove on (trips that are not cancelled),
    /// without duplicates and ordered by name.
    /// </summary>
    public List<string> GetStopNamesServedByVehicle(string licensePlate)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// All interventions of the trips of a safety operator, ordered by time.
    /// </summary>
    public List<Intervention> GetInterventionsOfOperator(int safetyOperatorId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Number and total duration of the interventions per weather condition, ordered by number
    /// (descending) and then by condition. Days without weather reading are not included.
    /// </summary>
    public List<WeatherStatistics> GetInterventionsByWeather()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// Every day with interventions, the number of interventions and the weather condition
    /// (null if there is no reading for that day), ordered by date.
    /// </summary>
    public List<DayWeather> GetInterventionDaysWithWeather()
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// The stop names of a line in the order of their position, separated by " -> ".
    /// Returns an empty string for an unknown line.
    /// </summary>
    public string GetRouteDescription(string lineNumber)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }

    /// <summary>
    /// The total duration of all interventions of a trip. TimeSpan.Zero if there are none.
    /// </summary>
    public TimeSpan GetTotalInterventionTime(int tripId)
    {
        // TODO: Add your implementation.
        throw new NotImplementedException();
    }
}
