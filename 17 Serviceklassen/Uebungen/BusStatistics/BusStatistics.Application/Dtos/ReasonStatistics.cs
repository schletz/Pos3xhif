using BusStatistics.Application.Model;

namespace BusStatistics.Application.Dtos;

public record ReasonStatistics(InterventionReason Reason, int Count, int TotalDurationSeconds);
