namespace BusStatistics.Application.Dtos;

public record LinePassengerStatistics(string LineNumber, int TripCount, int TotalPassengers, double AveragePassengers);
