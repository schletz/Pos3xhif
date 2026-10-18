using System;
using System.Collections.Generic;

namespace BusTickets.Application;

/// <summary>
/// The ticket machine in the bus. It sells tickets and adds up the revenue.
/// </summary>
public class TicketMachine
{
    private readonly List<Ticket> _soldTickets = new List<Ticket>();

    /// <summary>All tickets sold by this machine, in the order of purchase.</summary>
    public IReadOnlyList<Ticket> SoldTickets => _soldTickets;

    /// <summary>The sum of the prices of all sold tickets in euros.</summary>
    public decimal TotalRevenue
    {
        get
        {
            decimal total = 0m;
            foreach (Ticket ticket in _soldTickets)
            {
                total += ticket.Price;
            }
            return total;
        }
    }

    /// <summary>
    /// Sells a ticket of the given type. <paramref name="purchasedAt"/> is the moment of purchase.
    /// </summary>
    public Ticket Buy(TicketType type, DateTime purchasedAt, bool isReduced = false)
    {
        Ticket ticket = new Ticket(type, purchasedAt, isReduced);
        _soldTickets.Add(ticket);
        return ticket;
    }
}
