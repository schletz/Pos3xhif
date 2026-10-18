using System;

namespace BugHunt.Application;

/// <summary>
/// Calculates the parking fee of the garage. The rules are described in the README of this exercise.
/// </summary>
public static class ParkingFeeCalculator
{
    public const int FreeMinutes = 30;
    public const decimal PricePerHour = 2.50m;
    public const decimal MaxPerDay = 20m;

    /// <summary>
    /// Returns the fee in euros for a car that enters the garage at <paramref name="entry"/>
    /// and leaves it at <paramref name="exit"/>.
    /// </summary>
    public static decimal Calculate(DateTime entry, DateTime exit)
    {
        TimeSpan duration = exit - entry;
        if (duration.TotalMinutes < FreeMinutes) { return 0; }

        // Every started hour is charged.
        int hours = (int)duration.TotalHours + 1;
        return Math.Min(hours * PricePerHour, MaxPerDay);
    }
}
