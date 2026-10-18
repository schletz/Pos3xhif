using System;
using System.Collections.Generic;
using System.Globalization;

namespace Fleet.Application;

/// <summary>
/// Manages the vehicles of the company fleet: energy costs, trips and a short report.
/// </summary>
public class FleetService
{
    private readonly List<Vehicle> _vehicles = new List<Vehicle>();
    private readonly EnergyPrices _prices;

    /// <summary>
    /// Creates an empty fleet.
    /// </summary>
    /// <param name="prices">The energy prices for all cost calculations.</param>
    public FleetService(EnergyPrices prices)
    {
        _prices = prices;
    }

    /// <summary>
    /// Adds a vehicle to the fleet.
    /// </summary>
    /// <exception cref="ArgumentException">A vehicle with the same license plate is already in the fleet.</exception>
    public void Add(Vehicle vehicle)
    {
        foreach (Vehicle existing in _vehicles)
        {
            if (existing.LicensePlate == vehicle.LicensePlate)
            {
                throw new ArgumentException($"Vehicle {vehicle.LicensePlate} is already in the fleet.");
            }
        }
        _vehicles.Add(vehicle);
    }

    /// <summary>
    /// Returns the energy cost in euros for 100 km.
    /// </summary>
    /// <exception cref="ArgumentException">There is no vehicle with this license plate.</exception>
    public decimal CalculateCostPer100Km(string licensePlate)
    {
        Vehicle vehicle = FindVehicle(licensePlate);
        return GetCostPer100Km(vehicle);
    }

    /// <summary>
    /// Returns the energy cost in euros for one month, if every vehicle drives <paramref name="kmPerVehicle"/>.
    /// </summary>
    public decimal CalculateMonthlyCost(decimal kmPerVehicle)
    {
        decimal total = 0m;
        foreach (Vehicle vehicle in _vehicles)
        {
            total += GetCostPer100Km(vehicle) * kmPerVehicle / 100m;
        }
        return total;
    }

    /// <summary>
    /// Makes sure that the vehicle can drive the distance. If the range is too small, the tank is
    /// filled up or the battery is charged completely.
    /// </summary>
    /// <returns>The cost in euros for the fuel or the electricity. 0 if the range is already enough.</returns>
    /// <exception cref="ArgumentException">There is no vehicle with this license plate.</exception>
    public decimal PrepareForTrip(string licensePlate, decimal distanceKm)
    {
        Vehicle vehicle = FindVehicle(licensePlate);
        if (GetRangeKm(vehicle) >= distanceKm)
        {
            return 0m;
        }

        if (vehicle is ElectricCar electricCar)
        {
            decimal missingKWh = electricCar.BatteryCapacityKWh - electricCar.ChargeLevelKWh;
            electricCar.Charge(missingKWh);
            return missingKWh * _prices.ElectricityPerKWh;
        }

        decimal missingLiters = vehicle.TankCapacityLiters - vehicle.FuelLevelLiters;
        vehicle.Refuel(missingLiters);
        return missingLiters * _prices.FuelPerLiter;
    }

    /// <summary>
    /// Records a trip. The vehicle uses fuel or electricity for the distance.
    /// </summary>
    /// <exception cref="ArgumentException">There is no vehicle with this license plate.</exception>
    /// <exception cref="InvalidOperationException">The range of the vehicle is not enough.</exception>
    public void RecordTrip(string licensePlate, decimal distanceKm)
    {
        Vehicle vehicle = FindVehicle(licensePlate);
        vehicle.Drive(distanceKm);
    }

    /// <summary>
    /// Creates a report with one line per vehicle, for example
    /// "W-1001A: range 400 km, 10.40 EUR/100 km". The lines are separated by
    /// <see cref="Environment.NewLine"/>.
    /// </summary>
    public string CreateReport()
    {
        List<string> lines = new List<string>();
        foreach (Vehicle vehicle in _vehicles)
        {
            lines.Add(string.Format(CultureInfo.InvariantCulture, "{0}: range {1:0} km, {2:0.00} EUR/100 km",
                vehicle.LicensePlate, GetRangeKm(vehicle), GetCostPer100Km(vehicle)));
        }
        return string.Join(Environment.NewLine, lines);
    }

    private Vehicle FindVehicle(string licensePlate)
    {
        foreach (Vehicle vehicle in _vehicles)
        {
            if (vehicle.LicensePlate == licensePlate)
            {
                return vehicle;
            }
        }
        throw new ArgumentException($"Vehicle {licensePlate} is not in the fleet.");
    }

    private decimal GetRangeKm(Vehicle vehicle)
    {
        if (vehicle is ElectricCar electricCar)
        {
            return electricCar.ChargeLevelKWh / electricCar.ConsumptionKWhPer100Km * 100m;
        }
        return vehicle.FuelLevelLiters / vehicle.ConsumptionLitersPer100Km * 100m;
    }

    private decimal GetCostPer100Km(Vehicle vehicle)
    {
        if (vehicle is ElectricCar electricCar)
        {
            return electricCar.ConsumptionKWhPer100Km * _prices.ElectricityPerKWh;
        }
        return vehicle.ConsumptionLitersPer100Km * _prices.FuelPerLiter;
    }
}
