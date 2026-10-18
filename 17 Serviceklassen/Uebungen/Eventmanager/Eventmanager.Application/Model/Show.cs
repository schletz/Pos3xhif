using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Eventmanager.Application.Model;

/// <summary>
/// One performance of an event at a specific date and time.
/// </summary>
public class Show
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Show() { }
#pragma warning restore CS8618

    public Show(Event @event, DateTime date)
    {
        Event = @event;
        Date = date;
    }

    public int Id { get; set; }
    public Event Event { get; set; }
    public DateTime Date { get; set; }
    [JsonIgnore]
    public List<Contingent> Contingents { get; } = new();
}
