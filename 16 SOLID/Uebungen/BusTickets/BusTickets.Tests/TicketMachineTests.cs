using System;
using BusTickets.Application;
using Xunit;

namespace BusTickets.Tests;

public class TicketMachineTests
{
    private static readonly DateTime PurchaseTime = new DateTime(2026, 10, 17, 8, 15, 0);

    [Fact]
    public void Buy_ReducedDayPass_ReturnsTicketWithPurchaseData()
    {
        TicketMachine machine = new TicketMachine();

        Ticket ticket = machine.Buy(TicketType.DayPass, PurchaseTime, isReduced: true);

        Assert.Equal(PurchaseTime, ticket.PurchasedAt);
        Assert.True(ticket.IsReduced);
    }

    [Fact]
    public void Buy_ThreeTickets_AddsThemToSoldTickets()
    {
        TicketMachine machine = new TicketMachine();

        Ticket first = machine.Buy(TicketType.SingleRide, PurchaseTime);
        Ticket second = machine.Buy(TicketType.DayPass, PurchaseTime);
        Ticket third = machine.Buy(TicketType.MonthlyPass, PurchaseTime);

        Assert.Equal(new Ticket[] { first, second, third }, machine.SoldTickets);
    }

    [Fact]
    public void TotalRevenue_NoTicketsSold_ReturnsZero()
    {
        TicketMachine machine = new TicketMachine();
        Assert.Equal(0m, machine.TotalRevenue);
    }

    [Fact]
    public void TotalRevenue_ThreeTicketsSold_ReturnsSumOfPrices()
    {
        TicketMachine machine = new TicketMachine();
        machine.Buy(TicketType.SingleRide, PurchaseTime);
        machine.Buy(TicketType.DayPass, PurchaseTime, isReduced: true);
        machine.Buy(TicketType.MonthlyPass, PurchaseTime);

        // 2.40 + 2.80 + 42.00
        Assert.Equal(47.20m, machine.TotalRevenue);
    }
}
