using System;
using Fleet.Application;
using Xunit;

namespace Fleet.Tests;

public class VehicleTests
{
    [Fact]
    public void Constructor_FuelLevelAboveTankCapacity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CombustionCar("W-1001A", tankCapacityLiters: 50m, consumptionLitersPer100Km: 6.5m, fuelLevelLiters: 51m));
    }

    [Fact]
    public void Constructor_ChargeLevelAboveBatteryCapacity_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ElectricCar("W-2002B", batteryCapacityKWh: 60m, consumptionKWhPer100Km: 15m, chargeLevelKWh: 61m));
    }

    [Fact]
    public void Refuel_10Liters_IncreasesFuelLevel()
    {
        CombustionCar car = new CombustionCar("W-1001A", tankCapacityLiters: 50m, consumptionLitersPer100Km: 6.5m,
            fuelLevelLiters: 26m);

        car.Refuel(10m);

        Assert.Equal(36m, car.FuelLevelLiters);
    }

    [Fact]
    public void Refuel_MoreThanTankCapacity_ThrowsArgumentOutOfRangeException()
    {
        CombustionCar car = new CombustionCar("W-1001A", tankCapacityLiters: 50m, consumptionLitersPer100Km: 6.5m,
            fuelLevelLiters: 26m);

        Assert.Throws<ArgumentOutOfRangeException>(() => car.Refuel(30m));
    }

    [Fact]
    public void Charge_20KWh_IncreasesChargeLevel()
    {
        ElectricCar car = new ElectricCar("W-2002B", batteryCapacityKWh: 60m, consumptionKWhPer100Km: 15m,
            chargeLevelKWh: 30m);

        car.Charge(20m);

        Assert.Equal(50m, car.ChargeLevelKWh);
    }

    [Fact]
    public void Charge_MoreThanBatteryCapacity_ThrowsArgumentOutOfRangeException()
    {
        ElectricCar car = new ElectricCar("W-2002B", batteryCapacityKWh: 60m, consumptionKWhPer100Km: 15m,
            chargeLevelKWh: 30m);

        Assert.Throws<ArgumentOutOfRangeException>(() => car.Charge(40m));
    }
}
