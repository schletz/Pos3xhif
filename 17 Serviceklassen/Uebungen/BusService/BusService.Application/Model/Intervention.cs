using System;

namespace BusService.Application.Model;

/// <summary>
/// The safety operator took over control during a trip. Stop is null if the intervention
/// happened between two stops.
/// </summary>
public class Intervention
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Intervention() { }
#pragma warning restore CS8618

    public Intervention(Trip trip, Stop? stop, DateTime time, InterventionReason reason, int durationSeconds)
    {
        Trip = trip;
        Stop = stop;
        Time = time;
        Reason = reason;
        DurationSeconds = durationSeconds;
    }

    public int Id { get; set; }
    public Trip Trip { get; set; }
    public Stop? Stop { get; set; }
    public DateTime Time { get; set; }
    public InterventionReason Reason { get; set; }
    public int DurationSeconds { get; set; }
}
