using System;

namespace Fleet.Application;

/// <summary>
/// A vehicle of the company fleet. It has a tank and uses fuel when it drives.
/// </summary>
public abstract class Vehicle
{
    /// <summary>
    /// Creates a vehicle with a tank.
    /// </summary>
    /// <param name="licensePlate">The license plate, for example "W-1001A".</param>
    /// <param name="tankCapacityLiters">Size of the tank in liters.</param>
    /// <param name="consumptionLitersPer100Km">Average fuel consumption in liters per 100 km.</param>
    /// <param name="fuelLevelLiters">Fuel in the tank at the start.</param>
    /// <exception cref="ArgumentOutOfRangeException">The fuel level is negative or above the tank capacity.</exception>
    protected Vehicle(string licensePlate, decimal tankCapacityLiters, decimal consumptionLitersPer100Km,
        decimal fuelLevelLiters)
    {
        if (fuelLevelLiters < 0 || fuelLevelLiters > tankCapacityLiters)
        {
            throw new ArgumentOutOfRangeException(nameof(fuelLevelLiters),
                "The fuel level must be between 0 and the tank capacity.");
        }
        LicensePlate = licensePlate;
        TankCapacityLiters = tankCapacityLiters;
        ConsumptionLitersPer100Km = consumptionLitersPer100Km;
        FuelLevelLiters = fuelLevelLiters;
    }

    public string LicensePlate { get; }
    public decimal TankCapacityLiters { get; }
    public decimal ConsumptionLitersPer100Km { get; }
    public decimal FuelLevelLiters { get; private set; }

    /// <summary>
    /// Adds fuel to the tank.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The amount is negative or does not fit into the tank.</exception>
    public virtual void Refuel(decimal liters)
    {
        if (liters < 0 || FuelLevelLiters + liters > TankCapacityLiters)
        {
            throw new ArgumentOutOfRangeException(nameof(liters), "The fuel does not fit into the tank.");
        }
        FuelLevelLiters += liters;
    }

    /// <summary>
    /// Drives the distance and uses the fuel for it.
    /// </summary>
    /// <exception cref="InvalidOperationException">The fuel is not enough for the distance.</exception>
    public virtual void Drive(decimal distanceKm)
    {
        decimal neededLiters = distanceKm * ConsumptionLitersPer100Km / 100m;
        if (neededLiters > FuelLevelLiters)
        {
            throw new InvalidOperationException("Not enough fuel for this distance.");
        }
        FuelLevelLiters -= neededLiters;
    }
}
