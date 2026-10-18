namespace Dfa.Application;

/// <summary>
/// The events the traffic light reacts to. Encoded as characters they form the alphabet Σ.
/// </summary>
public enum TrafficLightEvent
{
    /// <summary>The time of the current phase has expired.</summary>
    Timer,

    /// <summary>A fault has been detected.</summary>
    Fault,

    /// <summary>A technician has repaired the traffic light.</summary>
    Repair
}
