#if EXTENSION
using System;
using BusTickets.Application;
using Xunit;

namespace BusTickets.Tests;

/// <summary>
/// Tests for step 2 of the exercise: the new class WeeklyPassTicket.
/// They are compiled only if the constant EXTENSION is defined in BusTickets.Tests.csproj.
/// </summary>
public class ExtensionTests
{
    // Monday, 12 October 2026, 08:15.
    private static readonly DateTime Monday = new DateTime(2026, 10, 12, 8, 15, 0);

    [Theory]
    [InlineData(false, 14.00)]
    [InlineData(true, 7.00)]
    public void Price_WeeklyPass_ReturnsTariff(bool isReduced, decimal expected)
    {
        Ticket ticket = new WeeklyPassTicket(Monday, isReduced);
        Assert.Equal(expected, ticket.Price);
    }

    [Fact]
    public void ExpiresAt_WeeklyPassBoughtOnMonday_ReturnsMidnightAfterSunday()
    {
        Ticket ticket = new WeeklyPassTicket(Monday, isReduced: false);
        Assert.Equal(new DateTime(2026, 10, 19, 0, 0, 0), ticket.ExpiresAt);
    }

    [Fact]
    public void ExpiresAt_WeeklyPassOverEndOfMonth_ReturnsDayInNextMonth()
    {
        Ticket ticket = new WeeklyPassTicket(new DateTime(2026, 10, 28, 18, 0, 0), isReduced: false);
        Assert.Equal(new DateTime(2026, 11, 4, 0, 0, 0), ticket.ExpiresAt);
    }

    [Theory]
    [InlineData(12, 8, 14, false)]
    [InlineData(12, 8, 15, true)]
    [InlineData(18, 23, 59, true)]
    [InlineData(19, 0, 0, false)]
    public void IsValidAt_WeeklyPassAtTime_ReturnsExpected(int day, int hour, int minute, bool expected)
    {
        Ticket ticket = new WeeklyPassTicket(Monday, isReduced: false);
        Assert.Equal(expected, ticket.IsValidAt(new DateTime(2026, 10, day, hour, minute, 0)));
    }

    [Fact]
    public void Description_WeeklyPass_ReturnsFirstAndLastDay()
    {
        Ticket ticket = new WeeklyPassTicket(Monday, isReduced: false);
        Assert.Equal("Weekly pass from 2026-10-12 to 2026-10-18", ticket.Description);
    }

    [Fact]
    public void Buy_WeeklyPass_ReturnsWeeklyPassTicket()
    {
        TicketMachine machine = new TicketMachine();

        Ticket ticket = machine.Buy(TicketType.WeeklyPass, Monday, isReduced: true);

        WeeklyPassTicket weeklyPass = Assert.IsType<WeeklyPassTicket>(ticket);
        Assert.True(weeklyPass.IsReduced);
    }

    [Fact]
    public void TotalRevenue_WeeklyPassAndSingleRideSold_ReturnsSumOfPrices()
    {
        TicketMachine machine = new TicketMachine();
        machine.Buy(TicketType.WeeklyPass, Monday);
        machine.Buy(TicketType.SingleRide, Monday);

        // 14.00 + 2.40
        Assert.Equal(16.40m, machine.TotalRevenue);
    }
}
#endif
