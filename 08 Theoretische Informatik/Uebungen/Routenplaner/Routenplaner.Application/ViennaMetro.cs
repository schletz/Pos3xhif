namespace Routenplaner.Application;

/// <summary>
/// Access to the simplified Vienna U-Bahn network in <c>Data/ubahn_wien.csv</c>.
/// The file is copied to the output folder during the build.
/// </summary>
public static class ViennaMetro
{
    /// <summary>Fixed time for changing between two lines at the same station, in minutes.</summary>
    public const int TransferMinutes = 4;

    /// <summary>Full path of the network file in the output folder.</summary>
    public static string DataFile => Path.Combine(AppContext.BaseDirectory, "Data", "ubahn_wien.csv");

    /// <summary>Reads the network file and builds the graph with <see cref="TransferMinutes"/>.</summary>
    public static TransitNetwork Load() => new(ConnectionReader.ReadFile(DataFile), TransferMinutes);
}
