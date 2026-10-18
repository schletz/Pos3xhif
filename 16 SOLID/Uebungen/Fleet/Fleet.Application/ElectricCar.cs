using System;

namespace Fleet.Application;

/// <summary>
/// A battery electric car. It is charged instead of refueled.
/// </summary>
public class ElectricCar : Vehicle
{
    /// <summary>
    /// Creates an electric car.
    /// </summary>
    /// <param name="licensePlate">The license plate, for example "W-2002B".</param>
    /// <param name="batteryCapacityKWh">Usable capacity of the battery in kWh.</param>
    /// <param name="consumptionKWhPer100Km">Average consumption in kWh per 100 km.</param>
    /// <param name="chargeLevelKWh">Energy in the battery at the start.</param>
    /// <exception cref="ArgumentOutOfRangeException">The charge level is negative or above the battery capacity.</exception>
    public ElectricCar(string licensePlate, decimal batteryCapacityKWh, decimal consumptionKWhPer100Km,
        decimal chargeLevelKWh)
        : base(licensePlate, 0m, 0m, 0m)
    {
        if (chargeLevelKWh < 0 || chargeLevelKWh > batteryCapacityKWh)
        {
            throw new ArgumentOutOfRangeException(nameof(chargeLevelKWh),
                "The charge level must be between 0 and the battery capacity.");
        }
        BatteryCapacityKWh = batteryCapacityKWh;
        ConsumptionKWhPer100Km = consumptionKWhPer100Km;
        ChargeLevelKWh = chargeLevelKWh;
    }

    public decimal BatteryCapacityKWh { get; }
    public decimal ConsumptionKWhPer100Km { get; }
    public decimal ChargeLevelKWh { get; private set; }

    /// <summary>
    /// An electric car has no tank. Use <see cref="Charge"/> instead.
    /// </summary>
    /// <exception cref="NotSupportedException">Always.</exception>
    public override void Refuel(decimal liters)
    {
        throw new NotSupportedException("An electric car cannot be refueled. Use Charge instead.");
    }

    /// <summary>
    /// Adds energy to the battery.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The amount is negative or does not fit into the battery.</exception>
    public void Charge(decimal kWh)
    {
        if (kWh < 0 || ChargeLevelKWh + kWh > BatteryCapacityKWh)
        {
            throw new ArgumentOutOfRangeException(nameof(kWh), "The energy does not fit into the battery.");
        }
        ChargeLevelKWh += kWh;
    }

    /// <summary>
    /// Drives the distance and uses the energy of the battery for it.
    /// </summary>
    /// <exception cref="InvalidOperationException">The charge is not enough for the distance.</exception>
    public override void Drive(decimal distanceKm)
    {
        decimal neededKWh = distanceKm * ConsumptionKWhPer100Km / 100m;
        if (neededKWh > ChargeLevelKWh)
        {
            throw new InvalidOperationException("Not enough charge for this distance.");
        }
        ChargeLevelKWh -= neededKWh;
    }
}
