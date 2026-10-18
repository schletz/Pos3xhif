using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// A small network for tests that do not depend on the Vienna data.
/// <code>
/// A: Alpha - Beta (2)          D: Alpha - Delta - Epsilon - Zeta - Omega (3 each)
/// B: Beta - Gamma (2)          E: Insel - Strand (1), not connected to the rest
/// C: Gamma - Omega (2)
/// </code>
/// From Alpha to Omega there are two routes: A, B, C with 3 stations and 2 transfers, or D with
/// 4 stations and no transfer.
/// </summary>
public static class MiniNetwork
{
    /// <summary>The sections of all lines.</summary>
    public static IReadOnlyList<Connection> Connections { get; } =
    [
        new("A", "Alpha", "Beta", 2),
        new("B", "Beta", "Gamma", 2),
        new("C", "Gamma", "Omega", 2),
        new("D", "Alpha", "Delta", 3),
        new("D", "Delta", "Epsilon", 3),
        new("D", "Epsilon", "Zeta", 3),
        new("D", "Zeta", "Omega", 3),
        new("E", "Insel", "Strand", 1),
    ];

    /// <summary>Builds the network with the given transfer time.</summary>
    public static TransitNetwork Create(int transferMinutes = 4) => new(Connections, transferMinutes);
}
