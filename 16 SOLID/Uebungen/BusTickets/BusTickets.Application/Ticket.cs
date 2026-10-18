using System;
using System.Globalization;

namespace BusTickets.Application;

/// <summary>
/// A ticket for the automated bus line. Price, validity and description depend on the ticket type.
/// </summary>
public class Ticket
{
    /// <summary>
    /// Creates a ticket. Tickets are sold with <see cref="TicketMachine.Buy"/>.
    /// </summary>
    public Ticket(TicketType type, DateTime purchasedAt, bool isReduced)
    {
        Type = type;
        PurchasedAt = purchasedAt;
        IsReduced = isReduced;
    }

    /// <summary>The type of the ticket.</summary>
    public TicketType Type { get; }

    /// <summary>The moment the ticket was bought. The ticket is valid from this moment on.</summary>
    public DateTime PurchasedAt { get; }

    /// <summary>True if the ticket was sold at the reduced price (children, seniors).</summary>
    public bool IsReduced { get; }

    /// <summary>The price in euros.</summary>
    public decimal Price
    {
        get
        {
            switch (Type)
            {
                case TicketType.SingleRide:
                    if (IsReduced) { return 1.20m; }
                    return 2.40m;
                case TicketType.DayPass:
                    if (IsReduced) { return 2.80m; }
                    return 5.60m;
                case TicketType.MonthlyPass:
                    if (IsReduced) { return 25.00m; }
                    return 42.00m;
                default:
                    throw new InvalidOperationException($"Unknown ticket type {Type}.");
            }
        }
    }

    /// <summary>The first moment at which the ticket is no longer valid.</summary>
    public DateTime ExpiresAt
    {
        get
        {
            switch (Type)
            {
                case TicketType.SingleRide:
                    return PurchasedAt.AddMinutes(60);
                case TicketType.DayPass:
                    // Valid until midnight of the day of purchase.
                    return PurchasedAt.Date.AddDays(1);
                case TicketType.MonthlyPass:
                    // Valid until the end of the calendar month of purchase.
                    DateTime firstDayOfMonth = new DateTime(PurchasedAt.Year, PurchasedAt.Month, 1);
                    return firstDayOfMonth.AddMonths(1);
                default:
                    throw new InvalidOperationException($"Unknown ticket type {Type}.");
            }
        }
    }

    /// <summary>The text that is printed on the ticket.</summary>
    public string Description
    {
        get
        {
            string description = "";
            if (Type == TicketType.SingleRide)
            {
                description = "Single ride, valid for 60 minutes";
            }
            else if (Type == TicketType.DayPass)
            {
                description = "Day pass for " + PurchasedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else if (Type == TicketType.MonthlyPass)
            {
                description = "Monthly pass for " + PurchasedAt.ToString("yyyy-MM", CultureInfo.InvariantCulture);
            }
            return description;
        }
    }

    /// <summary>
    /// Returns true if the ticket is valid at the given moment.
    /// </summary>
    public bool IsValidAt(DateTime time)
    {
        if (time < PurchasedAt) { return false; }

        switch (Type)
        {
            case TicketType.SingleRide:
                return (time - PurchasedAt).TotalMinutes < 60;
            case TicketType.DayPass:
                return time.Date == PurchasedAt.Date;
            case TicketType.MonthlyPass:
                return time.Year == PurchasedAt.Year && time.Month == PurchasedAt.Month;
            default:
                return false;
        }
    }
}
