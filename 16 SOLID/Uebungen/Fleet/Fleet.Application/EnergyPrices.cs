namespace Fleet.Application;

/// <summary>
/// Current energy prices the company pays for its fleet.
/// </summary>
/// <param name="FuelPerLiter">Price of one liter of fuel in euros.</param>
/// <param name="ElectricityPerKWh">Price of one kilowatt hour of electricity in euros.</param>
public record EnergyPrices(decimal FuelPerLiter, decimal ElectricityPerKWh);
