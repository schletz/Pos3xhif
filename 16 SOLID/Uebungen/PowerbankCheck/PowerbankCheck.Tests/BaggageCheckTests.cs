using System.Collections.Generic;
using PowerbankCheck.Application;
using Xunit;

namespace PowerbankCheck.Tests;

/// <summary>
/// Regression tests for <see cref="BaggageCheck"/>. They are green before the refactoring and
/// must stay green after it. Do not change them.
/// </summary>
public class BaggageCheckTests
{
    // 20000 mAh * 3.7 V / 1000 = 74 Wh
    private static Powerbank Small() => new Powerbank("Travel 20K", 20000, 3.7m);

    // 30000 mAh * 3.7 V / 1000 = 111 Wh
    private static Powerbank Large() => new Powerbank("Laptop 30K", 30000, 3.7m);

    // 45000 mAh * 3.7 V / 1000 = 166.5 Wh
    private static Powerbank Huge() => new Powerbank("Camping 45K", 45000, 3.7m);

    [Fact]
    public void CheckCarryOn_EmptyBag_ReturnsAllowed()
    {
        Passenger passenger = new Passenger("Anna Berger");

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Allowed, result);
    }

    [Fact]
    public void CheckCarryOn_WithPowerbank74Wh_ReturnsAllowed()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(Small());

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Allowed, result);
    }

    [Theory]
    [InlineData(20000, CheckResult.Allowed)]            // 20000 mAh * 5 V / 1000 = 100 Wh
    [InlineData(20100, CheckResult.ApprovalRequired)]   // 20100 mAh * 5 V / 1000 = 100.5 Wh
    [InlineData(32000, CheckResult.ApprovalRequired)]   // 32000 mAh * 5 V / 1000 = 160 Wh
    [InlineData(32100, CheckResult.Forbidden)]          // 32100 mAh * 5 V / 1000 = 160.5 Wh
    public void CheckCarryOn_AtWattHourLimits_ReturnsExpectedResult(int capacityMilliampHours, CheckResult expected)
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(new Powerbank("Test 5 V", capacityMilliampHours, 5m));

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void CheckCarryOn_WithLargeAndHugePowerbank_ReturnsForbidden()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(Large());
        passenger.CarryOn.Powerbanks.Add(Huge());

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Forbidden, result);
    }

    [Fact]
    public void CheckCarryOn_With2Powerbanks_ReturnsAllowed()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Small());

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Allowed, result);
    }

    [Fact]
    public void CheckCarryOn_With3Powerbanks_ReturnsForbidden()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Small());

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Forbidden, result);
    }

    [Theory]
    [InlineData("fireworks")]
    [InlineData("Camping Gas")]
    [InlineData("LIGHTER FLUID")]
    public void CheckCarryOn_WithForbiddenItem_ReturnsForbidden(string item)
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Items.Add("laptop");
        passenger.CarryOn.Items.Add(item);

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Forbidden, result);
    }

    [Fact]
    public void CheckCarryOn_WithAllowedItems_ReturnsAllowed()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Items.Add("laptop");
        passenger.CarryOn.Items.Add("camping chair");

        CheckResult result = new BaggageCheck().CheckCarryOn(passenger);

        Assert.Equal(CheckResult.Allowed, result);
    }

    [Fact]
    public void CheckCheckedBag_WithPowerbank74Wh_ReturnsForbidden()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CheckedBag.Powerbanks.Add(Small());

        CheckResult result = new BaggageCheck().CheckCheckedBag(passenger);

        Assert.Equal(CheckResult.Forbidden, result);
    }

    [Theory]
    [InlineData("Fireworks")]
    [InlineData("camping gas")]
    [InlineData("Lighter Fluid")]
    public void CheckCheckedBag_WithForbiddenItem_ReturnsForbidden(string item)
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CheckedBag.Items.Add("shoes");
        passenger.CheckedBag.Items.Add(item);

        CheckResult result = new BaggageCheck().CheckCheckedBag(passenger);

        Assert.Equal(CheckResult.Forbidden, result);
    }

    [Fact]
    public void CheckCheckedBag_WithAllowedItems_ReturnsAllowed()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CheckedBag.Items.Add("shoes");
        passenger.CheckedBag.Items.Add("camping chair");
        // A powerbank in the carry-on bag does not matter for the checked bag.
        passenger.CarryOn.Powerbanks.Add(Small());

        CheckResult result = new BaggageCheck().CheckCheckedBag(passenger);

        Assert.Equal(CheckResult.Allowed, result);
    }

    [Fact]
    public void CountRequiredApprovals_WithoutPassengers_Returns0()
    {
        int count = new BaggageCheck().CountRequiredApprovals(new List<Passenger>());

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountRequiredApprovals_With3Passengers_CountsOnlyCarryOnBetween100And160Wh()
    {
        Passenger anna = new Passenger("Anna Berger");
        anna.CarryOn.Powerbanks.Add(Large());                                // 111 Wh: counts
        anna.CarryOn.Powerbanks.Add(Small());                                // 74 Wh
        Passenger ben = new Passenger("Ben Huber");
        ben.CarryOn.Powerbanks.Add(new Powerbank("160 Wh", 32000, 5m));      // 160 Wh: counts
        ben.CheckedBag.Powerbanks.Add(Large());                              // checked bag: does not count
        Passenger clara = new Passenger("Clara Novak");
        clara.CarryOn.Powerbanks.Add(Huge());                                // 166.5 Wh
        clara.CarryOn.Powerbanks.Add(new Powerbank("100 Wh", 20000, 5m));    // 100 Wh

        int count = new BaggageCheck().CountRequiredApprovals(new List<Passenger> { anna, ben, clara });

        Assert.Equal(2, count);
    }

    [Fact]
    public void CreateReport_WithPowerbanksAndItems_ReturnsOneLinePerEntry()
    {
        Passenger passenger = new Passenger("Anna Berger");
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Large());
        passenger.CarryOn.Items.Add("laptop");
        passenger.CheckedBag.Powerbanks.Add(Small());
        passenger.CheckedBag.Items.Add("shoes");
        passenger.CheckedBag.Items.Add("Camping Gas");

        List<string> report = new BaggageCheck().CreateReport(passenger);

        List<string> expected = new List<string>
        {
            "Passenger: Anna Berger",
            "Carry-on: powerbank \"Travel 20K\", 74.0 Wh: allowed",
            "Carry-on: powerbank \"Laptop 30K\", 111.0 Wh: approval required",
            "Carry-on: item \"laptop\": allowed",
            "Checked bag: powerbank \"Travel 20K\", 74.0 Wh: forbidden",
            "Checked bag: item \"shoes\": allowed",
            "Checked bag: item \"Camping Gas\": forbidden",
            "Result carry-on: ApprovalRequired",
            "Result checked bag: Forbidden"
        };
        Assert.Equal(expected, report);
    }

    [Fact]
    public void CreateReport_With3PowerbanksAndHugePowerbank_ReportsTooManyPowerbanks()
    {
        Passenger passenger = new Passenger("Ben Huber");
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Small());
        passenger.CarryOn.Powerbanks.Add(Huge());

        List<string> report = new BaggageCheck().CreateReport(passenger);

        List<string> expected = new List<string>
        {
            "Passenger: Ben Huber",
            "Carry-on: powerbank \"Travel 20K\", 74.0 Wh: allowed",
            "Carry-on: powerbank \"Travel 20K\", 74.0 Wh: allowed",
            "Carry-on: powerbank \"Camping 45K\", 166.5 Wh: forbidden",
            "Carry-on: too many powerbanks (3, max. 2)",
            "Result carry-on: Forbidden",
            "Result checked bag: Allowed"
        };
        Assert.Equal(expected, report);
    }
}
