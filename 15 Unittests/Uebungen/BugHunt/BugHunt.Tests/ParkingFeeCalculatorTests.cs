using System;
using BugHunt.Application;
using Xunit;

namespace BugHunt.Tests;

public class ParkingFeeCalculatorTests
{
    [Fact]
    public void Calculate_With2Hours10Minutes_Returns7Point50()
    {
        DateTime entry = new DateTime(2026, 10, 12, 8, 0, 0);
        DateTime exit = new DateTime(2026, 10, 12, 10, 10, 0);

        decimal fee = ParkingFeeCalculator.Calculate(entry, exit);

        Assert.Equal(7.50m, fee);
    }
}
