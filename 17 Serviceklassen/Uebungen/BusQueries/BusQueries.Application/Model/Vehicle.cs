using System;
using System.Collections.Generic;

namespace BusQueries.Application.Model;

/// <summary>
/// A bus. Autonomous buses need a safety operator on board, conventional buses have a driver
/// who is not stored in this model.
/// </summary>
public class Vehicle
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Vehicle() { }
#pragma warning restore CS8618

    public Vehicle(string licensePlate, string model, int seats, bool isAutonomous, DateOnly commissionedOn)
    {
        LicensePlate = licensePlate;
        Model = model;
        Seats = seats;
        IsAutonomous = isAutonomous;
        CommissionedOn = commissionedOn;
    }

    public int Id { get; set; }
    public string LicensePlate { get; set; }
    public string Model { get; set; }
    public int Seats { get; set; }
    public bool IsAutonomous { get; set; }
    public DateOnly CommissionedOn { get; set; }
    public List<Trip> Trips { get; } = new();
}
