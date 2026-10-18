namespace Routenplaner.Application;

/// <summary>
/// One line of the network file: a section of a line between two neighbouring stations.
/// The section can be travelled in both directions.
/// </summary>
/// <param name="Line">Name of the line, e.g. <c>U1</c>.</param>
/// <param name="From">First station of the section.</param>
/// <param name="To">Second station of the section.</param>
/// <param name="Minutes">Travel time between the two stations in whole minutes (at least 1).</param>
public sealed record Connection(string Line, string From, string To, int Minutes);
