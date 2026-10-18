namespace RegexLogAnalyzer.Application;

/// <summary>
/// Checks Austrian IBANs.
/// </summary>
public static partial class IbanValidator
{
    /// <summary>
    /// Checks the format of an Austrian IBAN (no check digit calculation).
    /// </summary>
    /// <param name="iban">The IBAN, compact or in groups of four.</param>
    /// <returns><c>true</c> if the format is valid.</returns>
    public static bool IsValidFormat(string iban) => throw new NotImplementedException();

    /// <summary>
    /// Optional extra: checks the format and the check digits (ISO 13616, modulo 97).
    /// </summary>
    /// <param name="iban">The IBAN, compact or in groups of four.</param>
    /// <returns><c>true</c> if the format is valid and the remainder is 1.</returns>
    public static bool HasValidChecksum(string iban) => throw new NotImplementedException();
}
