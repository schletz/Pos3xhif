namespace RegexLogAnalyzer.Application;

/// <summary>
/// Checks Austrian licence plates (normal plates, simplified rule from the README).
/// </summary>
public static partial class LicensePlateValidator
{
    /// <summary>
    /// Checks whether <paramref name="plate"/> is a valid Austrian licence plate like <c>W-12345A</c>.
    /// </summary>
    /// <param name="plate">The plate with a hyphen between district code and registration part.</param>
    /// <returns><c>true</c> if the plate follows the rule.</returns>
    public static bool IsValid(string plate) => throw new NotImplementedException();
}
