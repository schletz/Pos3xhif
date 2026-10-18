namespace PowerbankCheck.Application;

/// <summary>
/// A powerbank as it is printed on its label.
/// </summary>
public class Powerbank
{
    /// <summary>
    /// Creates a powerbank with the values from its label.
    /// </summary>
    /// <param name="name">A name to find the powerbank in the report.</param>
    /// <param name="capacityMilliampHours">The capacity in milliampere hours (mAh).</param>
    /// <param name="voltage">The nominal voltage in volts (V).</param>
    public Powerbank(string name, int capacityMilliampHours, decimal voltage)
    {
        Name = name;
        CapacityMilliampHours = capacityMilliampHours;
        Voltage = voltage;
    }

    /// <summary>A name to find the powerbank in the report.</summary>
    public string Name { get; }

    /// <summary>The capacity in milliampere hours (mAh).</summary>
    public int CapacityMilliampHours { get; }

    /// <summary>The nominal voltage in volts (V).</summary>
    public decimal Voltage { get; }
}
