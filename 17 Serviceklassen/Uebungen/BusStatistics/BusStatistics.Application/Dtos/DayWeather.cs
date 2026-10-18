using System;

namespace BusStatistics.Application.Dtos;

/// <summary>
/// A day with interventions. Condition is null if there is no weather reading for that day.
/// </summary>
public record DayWeather(DateOnly Date, int InterventionCount, string? Condition);
