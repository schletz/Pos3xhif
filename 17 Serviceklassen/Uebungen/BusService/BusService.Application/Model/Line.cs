using System.Collections.Generic;

namespace BusService.Application.Model;

/// <summary>
/// A bus line, e.g. "A1 Bahnhof - FH Campus". The stops are stored in <see cref="LineStops"/>.
/// </summary>
public class Line
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Line() { }
#pragma warning restore CS8618

    public Line(string number, string name)
    {
        Number = number;
        Name = name;
    }

    public int Id { get; set; }
    public string Number { get; set; }
    public string Name { get; set; }
    public List<LineStop> LineStops { get; } = new();
    public List<Trip> Trips { get; } = new();
}
