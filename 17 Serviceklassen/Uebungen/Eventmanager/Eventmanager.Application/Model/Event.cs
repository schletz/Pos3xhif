using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Eventmanager.Application.Model;

/// <summary>
/// An event (e.g. a music genre night) that takes place in several shows.
/// </summary>
public class Event
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Event() { }
#pragma warning restore CS8618

    public Event(string name)
    {
        Name = name;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    [JsonIgnore]
    public List<Show> Shows { get; } = new();
}
