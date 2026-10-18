namespace Dfa.Application;

/// <summary>
/// The phases of a traffic light (Austrian sequence) plus the fault mode.
/// </summary>
public enum TrafficLightState
{
    /// <summary>Red.</summary>
    Red,

    /// <summary>Red and yellow at the same time.</summary>
    RedYellow,

    /// <summary>Green.</summary>
    Green,

    /// <summary>Green, blinking.</summary>
    GreenBlinking,

    /// <summary>Yellow.</summary>
    Yellow,

    /// <summary>Yellow blinking: the traffic light is out of order.</summary>
    YellowBlinking
}
