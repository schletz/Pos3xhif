namespace Fleet.Application;

/// <summary>
/// A car with a petrol or diesel engine.
/// </summary>
public class CombustionCar : Vehicle
{
    /// <summary>
    /// Creates a car with a tank.
    /// </summary>
    /// <param name="licensePlate">The license plate, for example "W-1001A".</param>
    /// <param name="tankCapacityLiters">Size of the tank in liters.</param>
    /// <param name="consumptionLitersPer100Km">Average fuel consumption in liters per 100 km.</param>
    /// <param name="fuelLevelLiters">Fuel in the tank at the start.</param>
    public CombustionCar(string licensePlate, decimal tankCapacityLiters, decimal consumptionLitersPer100Km,
        decimal fuelLevelLiters)
        : base(licensePlate, tankCapacityLiters, consumptionLitersPer100Km, fuelLevelLiters)
    {
    }
}
