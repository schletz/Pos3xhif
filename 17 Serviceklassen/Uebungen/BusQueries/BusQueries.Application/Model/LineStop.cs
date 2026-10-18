namespace BusQueries.Application.Model;

/// <summary>
/// A stop of a line. Position 1 is the first stop, MinutesFromStart is the travel time
/// from the first stop (0 for the first stop).
/// </summary>
public class LineStop
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected LineStop() { }
#pragma warning restore CS8618

    public LineStop(Line line, Stop stop, int position, int minutesFromStart)
    {
        Line = line;
        Stop = stop;
        Position = position;
        MinutesFromStart = minutesFromStart;
    }

    public int Id { get; set; }
    public Line Line { get; set; }
    public Stop Stop { get; set; }
    public int Position { get; set; }
    public int MinutesFromStart { get; set; }
}
