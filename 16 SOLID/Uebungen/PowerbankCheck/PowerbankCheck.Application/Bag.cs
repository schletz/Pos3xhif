using System.Collections.Generic;

namespace PowerbankCheck.Application;

/// <summary>
/// A bag of a passenger, either the carry-on bag or the checked bag.
/// </summary>
public class Bag
{
    /// <summary>The powerbanks in this bag.</summary>
    public List<Powerbank> Powerbanks { get; } = new List<Powerbank>();

    /// <summary>All other items in this bag, for example "laptop" or "shoes".</summary>
    public List<string> Items { get; } = new List<string>();
}
