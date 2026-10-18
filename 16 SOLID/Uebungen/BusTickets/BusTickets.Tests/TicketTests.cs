using System;
using BusTickets.Application;
using Xunit;

namespace BusTickets.Tests;

public class TicketTests
{
    // Saturday, 17 October 2026, 08:15.
    private static readonly DateTime PurchaseTime = new DateTime(2026, 10, 17, 8, 15, 0);

    private static Ticket Buy(TicketType type, DateTime purchasedAt, bool isReduced = false)
    {
        TicketMachine machine = new TicketMachine();
        return machine.Buy(type, purchasedAt, isReduced);
    }

    [Theory]
    [InlineData(TicketType.SingleRide, false, 2.40)]
    [InlineData(TicketType.SingleRide, true, 1.20)]
    [InlineData(TicketType.DayPass, false, 5.60)]
    [InlineData(TicketType.DayPass, true, 2.80)]
    [InlineData(TicketType.MonthlyPass, false, 42.00)]
    [InlineData(TicketType.MonthlyPass, true, 25.00)]
    public void Price_TypeAndReduction_ReturnsTariff(TicketType type, bool isReduced, decimal expected)
    {
        Ticket ticket = Buy(type, PurchaseTime, isReduced);
        Assert.Equal(expected, ticket.Price);
    }

    [Fact]
    public void ExpiresAt_SingleRide_Returns60MinutesAfterPurchase()
    {
        Ticket ticket = Buy(TicketType.SingleRide, PurchaseTime);
        Assert.Equal(new DateTime(2026, 10, 17, 9, 15, 0), ticket.ExpiresAt);
    }

    [Fact]
    public void ExpiresAt_DayPass_ReturnsNextMidnight()
    {
        Ticket ticket = Buy(TicketType.DayPass, PurchaseTime);
        Assert.Equal(new DateTime(2026, 10, 18, 0, 0, 0), ticket.ExpiresAt);
    }

    [Fact]
    public void ExpiresAt_MonthlyPass_ReturnsFirstDayOfNextMonth()
    {
        Ticket ticket = Buy(TicketType.MonthlyPass, PurchaseTime);
        Assert.Equal(new DateTime(2026, 11, 1, 0, 0, 0), ticket.ExpiresAt);
    }

    [Fact]
    public void ExpiresAt_MonthlyPassBoughtInDecember_ReturnsFirstDayOfNextYear()
    {
        Ticket ticket = Buy(TicketType.MonthlyPass, new DateTime(2026, 12, 5, 17, 30, 0));
        Assert.Equal(new DateTime(2027, 1, 1, 0, 0, 0), ticket.ExpiresAt);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(59, true)]
    [InlineData(60, false)]
    public void IsValidAt_SingleRideMinutesAfterPurchase_ReturnsExpected(int minutes, bool expected)
    {
        Ticket ticket = Buy(TicketType.SingleRide, PurchaseTime);
        Assert.Equal(expected, ticket.IsValidAt(PurchaseTime.AddMinutes(minutes)));
    }

    [Theory]
    [InlineData(17, 8, 14, false)]
    [InlineData(17, 8, 15, true)]
    [InlineData(17, 23, 59, true)]
    [InlineData(18, 0, 0, false)]
    public void IsValidAt_DayPassAtTime_ReturnsExpected(int day, int hour, int minute, bool expected)
    {
        Ticket ticket = Buy(TicketType.DayPass, PurchaseTime);
        Assert.Equal(expected, ticket.IsValidAt(new DateTime(2026, 10, day, hour, minute, 0)));
    }

    [Theory]
    [InlineData(10, 17, 8, 14, false)]
    [InlineData(10, 17, 8, 15, true)]
    [InlineData(10, 31, 23, 59, true)]
    [InlineData(11, 1, 0, 0, false)]
    public void IsValidAt_MonthlyPassAtTime_ReturnsExpected(int month, int day, int hour, int minute, bool expected)
    {
        Ticket ticket = Buy(TicketType.MonthlyPass, PurchaseTime);
        Assert.Equal(expected, ticket.IsValidAt(new DateTime(2026, month, day, hour, minute, 0)));
    }

    [Fact]
    public void Description_SingleRide_ReturnsDuration()
    {
        Ticket ticket = Buy(TicketType.SingleRide, PurchaseTime);
        Assert.Equal("Single ride, valid for 60 minutes", ticket.Description);
    }

    [Fact]
    public void Description_DayPass_ReturnsDayOfPurchase()
    {
        Ticket ticket = Buy(TicketType.DayPass, PurchaseTime);
        Assert.Equal("Day pass for 2026-10-17", ticket.Description);
    }

    [Fact]
    public void Description_MonthlyPass_ReturnsMonthOfPurchase()
    {
        Ticket ticket = Buy(TicketType.MonthlyPass, PurchaseTime);
        Assert.Equal("Monthly pass for 2026-10", ticket.Description);
    }
}
