using System.Collections.Generic;

namespace BusService.Application.Model;

/// <summary>
/// A bus stop. One stop can be served by several lines.
/// </summary>
public class Stop
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Stop() { }
#pragma warning restore CS8618

    public Stop(string name, string municipality)
    {
        Name = name;
        Municipality = municipality;
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Municipality { get; set; }
    public List<LineStop> LineStops { get; } = new();
}
