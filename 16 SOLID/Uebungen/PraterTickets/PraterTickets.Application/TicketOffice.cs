using System;

namespace PraterTickets.Application;

/// <summary>
/// Calculates ticket prices for the rides at the Prater.
/// The tariff rules are described in the README of this exercise.
/// </summary>
public class TicketOffice
{
    /// <summary>
    /// Returns the total price in euros for one person who books <paramref name="rides"/> rides
    /// on <paramref name="ride"/>.
    /// </summary>
    /// <param name="ride">The ride the person wants to use.</param>
    /// <param name="age">Age of the person in years (0 to 120).</param>
    /// <param name="heightCm">Height of the person in centimeters.</param>
    /// <param name="rides">Number of rides (at least 1).</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="age"/>, <paramref name="heightCm"/> or <paramref name="rides"/> is out of range.
    /// </exception>
    /// <exception cref="ArgumentException">The person is smaller than the minimum height of the ride.</exception>
    public decimal CalculatePrice(Ride ride, int age, int heightCm, int rides)
    {
        if (age < 0 || age > 120)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be between 0 and 120.");
        }
        if (heightCm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightCm), "Height must be positive.");
        }
        if (rides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rides), "At least one ride is required.");
        }

        decimal total;
        if (ride == Ride.FerrisWheel)
        {
            // Toddlers ride the Ferris wheel for free.
            if (age < 3)
            {
                return 0m;
            }
            decimal price = 12.00m;
            // children and seniors get a discount
            if (age < 15)
            {
                price = price - price * 0.50m;
            }
            else if (age >= 65)
            {
                price = price - price * 0.30m;
            }
            total = price * rides;
            // multi-ride ticket
            if (rides >= 5)
            {
                total = total - total * 0.10m;
            }
        }
        else if (ride == Ride.RollerCoaster)
        {
            if (heightCm < 120)
            {
                throw new ArgumentException("Minimum height for the roller coaster is 120 cm.", nameof(heightCm));
            }
            decimal price = 8.00m;
            // children get a discount, seniors pay the full price
            if (age <= 14)
            {
                price = price - price * 0.25m;
            }
            total = price * rides;
            // multi-ride ticket
            if (rides >= 5)
            {
                total = total - total * 0.20m;
            }
        }
        else if (ride == Ride.GhostTrain)
        {
            if (heightCm < 100)
            {
                throw new ArgumentException("Minimum height for the ghost train is 100 cm.", nameof(heightCm));
            }
            decimal ticketPrice = 6.00m;
            if (age < 15)
            {
                ticketPrice = ticketPrice - ticketPrice * 0.30m;
            }
            if (age > 64)
            {
                ticketPrice = ticketPrice - ticketPrice * 0.30m;
            }
            total = ticketPrice * rides;
            if (rides >= 5)
            {
                total = total - total * 0.20m;
            }
        }
        else
        {
            throw new ArgumentException("Unknown ride.", nameof(ride));
        }
        return total;
    }
}
