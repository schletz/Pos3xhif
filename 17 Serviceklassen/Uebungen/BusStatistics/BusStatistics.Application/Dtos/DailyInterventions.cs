using System;

namespace BusStatistics.Application.Dtos;

public record DailyInterventions(DateOnly Date, int Count, int TotalDurationSeconds);
