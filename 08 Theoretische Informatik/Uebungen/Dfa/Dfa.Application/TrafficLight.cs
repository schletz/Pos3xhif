namespace Dfa.Application;

/// <summary>
/// Builds the traffic light automaton and encodes events as characters.
/// </summary>
public static class TrafficLight
{
    /// <summary>
    /// Creates a DFA over the encoded <see cref="TrafficLightEvent"/> values (see <see cref="Encode"/>).
    /// It accepts a sequence of events if every event is allowed and the light is red afterwards.
    /// </summary>
    public static Dfa<TrafficLightState> Create()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Encodes a sequence of events as a word over the alphabet of the automaton
    /// (Timer → <c>t</c>, Fault → <c>f</c>, Repair → <c>r</c>).
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="events"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">An event is not a defined enum value.</exception>
    public static string Encode(params IEnumerable<TrafficLightEvent> events)
    {
        throw new NotImplementedException();
    }
}
