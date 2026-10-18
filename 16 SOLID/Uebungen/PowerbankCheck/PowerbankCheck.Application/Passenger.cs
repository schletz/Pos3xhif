namespace PowerbankCheck.Application;

/// <summary>
/// A passenger with a carry-on bag and a checked bag.
/// </summary>
public class Passenger
{
    /// <summary>
    /// Creates a passenger with two empty bags.
    /// </summary>
    public Passenger(string name)
    {
        Name = name;
    }

    /// <summary>The name of the passenger.</summary>
    public string Name { get; }

    /// <summary>The bag the passenger takes into the cabin.</summary>
    public Bag CarryOn { get; } = new Bag();

    /// <summary>The bag that travels in the cargo hold.</summary>
    public Bag CheckedBag { get; } = new Bag();
}
