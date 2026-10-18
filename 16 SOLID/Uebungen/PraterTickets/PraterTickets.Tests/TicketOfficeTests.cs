using System;
using PraterTickets.Application;
using Xunit;

namespace PraterTickets.Tests;

/// <summary>
/// Regression tests for <see cref="TicketOffice"/>. They are green before the refactoring
/// and must stay green after every step.
/// </summary>
public class TicketOfficeTests
{
    private const int AdultAge = 30;
    private const int AdultHeight = 175;

    private readonly TicketOffice _office = new TicketOffice();

    // ---------- Ferris wheel: 12.00, children -50 %, seniors -30 %, from 5 rides -10 % ----------

    [Theory]
    [InlineData(2, 0.00)]
    [InlineData(3, 6.00)]
    [InlineData(14, 6.00)]
    [InlineData(15, 12.00)]
    [InlineData(64, 12.00)]
    [InlineData(65, 8.40)]
    public void CalculatePrice_FerrisWheelOneRide_ReturnsPriceForAge(int age, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.FerrisWheel, age, 100, 1);
        Assert.Equal(expected, price);
    }

    [Fact]
    public void CalculatePrice_FerrisWheelSmallChild_HasNoMinimumHeight()
    {
        decimal price = _office.CalculatePrice(Ride.FerrisWheel, 4, 60, 1);
        Assert.Equal(6.00m, price);
    }

    [Theory]
    [InlineData(AdultAge, 4, 48.00)]
    [InlineData(AdultAge, 5, 54.00)]
    [InlineData(70, 5, 37.80)]
    [InlineData(10, 6, 32.40)]
    [InlineData(2, 5, 0.00)]
    public void CalculatePrice_FerrisWheelSeveralRides_AppliesMultiRideDiscountFrom5Rides(int age, int rides, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.FerrisWheel, age, 140, rides);
        Assert.Equal(expected, price);
    }

    // ---------- Roller coaster: 8.00, min. 120 cm, children -25 %, no senior discount, from 5 rides -20 % ----------

    [Theory]
    [InlineData(10, 6.00)]
    [InlineData(14, 6.00)]
    [InlineData(15, 8.00)]
    [InlineData(65, 8.00)]
    [InlineData(80, 8.00)]
    public void CalculatePrice_RollerCoasterOneRide_ReturnsPriceForAge(int age, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.RollerCoaster, age, 150, 1);
        Assert.Equal(expected, price);
    }

    [Theory]
    [InlineData(AdultAge, 4, 32.00)]
    [InlineData(AdultAge, 5, 32.00)]
    [InlineData(12, 5, 24.00)]
    [InlineData(70, 10, 64.00)]
    public void CalculatePrice_RollerCoasterSeveralRides_AppliesMultiRideDiscountFrom5Rides(int age, int rides, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.RollerCoaster, age, 150, rides);
        Assert.Equal(expected, price);
    }

    [Fact]
    public void CalculatePrice_RollerCoasterWithExactly120Cm_IsAllowed()
    {
        decimal price = _office.CalculatePrice(Ride.RollerCoaster, 12, 120, 1);
        Assert.Equal(6.00m, price);
    }

    [Fact]
    public void CalculatePrice_RollerCoasterWith119Cm_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => _office.CalculatePrice(Ride.RollerCoaster, 12, 119, 1));
        Assert.Equal("heightCm", ex.ParamName);
    }

    // ---------- Ghost train: 6.00, min. 100 cm, children -30 %, seniors -30 %, from 5 rides -20 % ----------

    [Theory]
    [InlineData(8, 4.20)]
    [InlineData(14, 4.20)]
    [InlineData(15, 6.00)]
    [InlineData(64, 6.00)]
    [InlineData(65, 4.20)]
    public void CalculatePrice_GhostTrainOneRide_ReturnsPriceForAge(int age, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.GhostTrain, age, 130, 1);
        Assert.Equal(expected, price);
    }

    [Theory]
    [InlineData(AdultAge, 4, 24.00)]
    [InlineData(AdultAge, 5, 24.00)]
    [InlineData(10, 5, 16.80)]
    [InlineData(70, 7, 23.52)]
    public void CalculatePrice_GhostTrainSeveralRides_AppliesMultiRideDiscountFrom5Rides(int age, int rides, decimal expected)
    {
        decimal price = _office.CalculatePrice(Ride.GhostTrain, age, 130, rides);
        Assert.Equal(expected, price);
    }

    [Fact]
    public void CalculatePrice_GhostTrainWithExactly100Cm_IsAllowed()
    {
        decimal price = _office.CalculatePrice(Ride.GhostTrain, 6, 100, 1);
        Assert.Equal(4.20m, price);
    }

    [Fact]
    public void CalculatePrice_GhostTrainWith99Cm_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => _office.CalculatePrice(Ride.GhostTrain, 6, 99, 1));
        Assert.Equal("heightCm", ex.ParamName);
    }

    // ---------- Invalid input ----------

    [Theory]
    [InlineData(-1)]
    [InlineData(121)]
    public void CalculatePrice_WithInvalidAge_ThrowsArgumentOutOfRangeException(int age)
    {
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _office.CalculatePrice(Ride.FerrisWheel, age, AdultHeight, 1));
        Assert.Equal("age", ex.ParamName);
    }

    [Fact]
    public void CalculatePrice_WithHeight0_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _office.CalculatePrice(Ride.FerrisWheel, AdultAge, 0, 1));
        Assert.Equal("heightCm", ex.ParamName);
    }

    [Fact]
    public void CalculatePrice_With0Rides_ThrowsArgumentOutOfRangeException()
    {
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => _office.CalculatePrice(Ride.GhostTrain, AdultAge, AdultHeight, 0));
        Assert.Equal("rides", ex.ParamName);
    }

    [Fact]
    public void CalculatePrice_WithUnknownRide_ThrowsArgumentException()
    {
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => _office.CalculatePrice((Ride)99, AdultAge, AdultHeight, 1));
        Assert.Equal("ride", ex.ParamName);
    }
}
