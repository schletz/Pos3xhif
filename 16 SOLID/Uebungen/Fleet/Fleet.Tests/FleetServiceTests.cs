using System;
using Fleet.Application;
using Xunit;

namespace Fleet.Tests;

public class FleetServiceTests
{
    // 1.60 EUR per liter fuel, 0.30 EUR per kWh electricity.
    private static readonly EnergyPrices Prices = new EnergyPrices(FuelPerLiter: 1.60m, ElectricityPerKWh: 0.30m);

    // Range: 26 l / 6.5 l per 100 km = 400 km. Cost: 6.5 l * 1.60 EUR = 10.40 EUR per 100 km.
    private static CombustionCar CreateCombustionCar() =>
        new CombustionCar("W-1001A", tankCapacityLiters: 50m, consumptionLitersPer100Km: 6.5m, fuelLevelLiters: 26m);

    // Range: 30 kWh / 15 kWh per 100 km = 200 km. Cost: 15 kWh * 0.30 EUR = 4.50 EUR per 100 km.
    private static ElectricCar CreateElectricCar() =>
        new ElectricCar("W-2002B", batteryCapacityKWh: 60m, consumptionKWhPer100Km: 15m, chargeLevelKWh: 30m);

    [Fact]
    public void Add_DuplicateLicensePlate_ThrowsArgumentException()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());

        Assert.Throws<ArgumentException>(() => fleet.Add(CreateCombustionCar()));
    }

    [Fact]
    public void CalculateCostPer100Km_CombustionCar_Returns10Point40()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());

        Assert.Equal(10.40m, fleet.CalculateCostPer100Km("W-1001A"));
    }

    [Fact]
    public void CalculateCostPer100Km_ElectricCar_Returns4Point50()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateElectricCar());

        Assert.Equal(4.50m, fleet.CalculateCostPer100Km("W-2002B"));
    }

    [Fact]
    public void CalculateCostPer100Km_UnknownLicensePlate_ThrowsArgumentException()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());

        Assert.Throws<ArgumentException>(() => fleet.CalculateCostPer100Km("W-9999X"));
    }

    [Fact]
    public void CalculateMonthlyCost_EmptyFleet_ReturnsZero()
    {
        FleetService fleet = new FleetService(Prices);

        Assert.Equal(0m, fleet.CalculateMonthlyCost(1500m));
    }

    [Fact]
    public void CalculateMonthlyCost_OneCarOfEachType_Returns223Point50()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());
        fleet.Add(CreateElectricCar());

        // 1500 km = 15 * 100 km: 15 * 10.40 + 15 * 4.50 = 156.00 + 67.50
        Assert.Equal(223.50m, fleet.CalculateMonthlyCost(1500m));
    }

    [Fact]
    public void PrepareForTrip_CombustionCarWithEnoughFuel_ReturnsZeroAndKeepsFuel()
    {
        FleetService fleet = new FleetService(Prices);
        CombustionCar car = CreateCombustionCar();
        fleet.Add(car);

        decimal cost = fleet.PrepareForTrip("W-1001A", 300m);

        Assert.Equal(0m, cost);
        Assert.Equal(26m, car.FuelLevelLiters);
    }

    [Fact]
    public void PrepareForTrip_CombustionCarWithExactRange_ReturnsZero()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());

        Assert.Equal(0m, fleet.PrepareForTrip("W-1001A", 400m));
    }

    [Fact]
    public void PrepareForTrip_CombustionCarWithTooLittleFuel_FillsTankAndReturns38Point40()
    {
        FleetService fleet = new FleetService(Prices);
        CombustionCar car = CreateCombustionCar();
        fleet.Add(car);

        decimal cost = fleet.PrepareForTrip("W-1001A", 500m);

        // 50 l - 26 l = 24 l, 24 * 1.60 EUR = 38.40 EUR
        Assert.Equal(38.40m, cost);
        Assert.Equal(50m, car.FuelLevelLiters);
    }

    [Fact]
    public void PrepareForTrip_ElectricCarWithEnoughCharge_ReturnsZeroAndKeepsCharge()
    {
        FleetService fleet = new FleetService(Prices);
        ElectricCar car = CreateElectricCar();
        fleet.Add(car);

        decimal cost = fleet.PrepareForTrip("W-2002B", 150m);

        Assert.Equal(0m, cost);
        Assert.Equal(30m, car.ChargeLevelKWh);
    }

    [Fact]
    public void PrepareForTrip_ElectricCarWithTooLittleCharge_ChargesFullyAndReturns9()
    {
        FleetService fleet = new FleetService(Prices);
        ElectricCar car = CreateElectricCar();
        fleet.Add(car);

        decimal cost = fleet.PrepareForTrip("W-2002B", 250m);

        // 60 kWh - 30 kWh = 30 kWh, 30 * 0.30 EUR = 9.00 EUR
        Assert.Equal(9.00m, cost);
        Assert.Equal(60m, car.ChargeLevelKWh);
    }

    [Fact]
    public void RecordTrip_CombustionCar200Km_Uses13Liters()
    {
        FleetService fleet = new FleetService(Prices);
        CombustionCar car = CreateCombustionCar();
        fleet.Add(car);

        fleet.RecordTrip("W-1001A", 200m);

        // 2 * 6.5 l = 13 l, 26 l - 13 l = 13 l
        Assert.Equal(13m, car.FuelLevelLiters);
    }

    [Fact]
    public void RecordTrip_ElectricCar100Km_Uses15KWh()
    {
        FleetService fleet = new FleetService(Prices);
        ElectricCar car = CreateElectricCar();
        fleet.Add(car);

        fleet.RecordTrip("W-2002B", 100m);

        Assert.Equal(15m, car.ChargeLevelKWh);
    }

    [Fact]
    public void RecordTrip_ElectricCarBeyondRange_ThrowsInvalidOperationException()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateElectricCar());

        Assert.Throws<InvalidOperationException>(() => fleet.RecordTrip("W-2002B", 300m));
    }

    [Fact]
    public void CreateReport_OneCarOfEachType_ReturnsOneLinePerVehicle()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateCombustionCar());
        fleet.Add(CreateElectricCar());

        string[] lines = fleet.CreateReport().Split(Environment.NewLine);

        Assert.Equal(2, lines.Length);
        Assert.Equal("W-1001A: range 400 km, 10.40 EUR/100 km", lines[0]);
        Assert.Equal("W-2002B: range 200 km, 4.50 EUR/100 km", lines[1]);
    }

    [Fact]
    public void CreateReport_AfterTrip_ShowsReducedRange()
    {
        FleetService fleet = new FleetService(Prices);
        fleet.Add(CreateElectricCar());

        fleet.RecordTrip("W-2002B", 50m);

        Assert.Equal("W-2002B: range 150 km, 4.50 EUR/100 km", fleet.CreateReport());
    }
}
