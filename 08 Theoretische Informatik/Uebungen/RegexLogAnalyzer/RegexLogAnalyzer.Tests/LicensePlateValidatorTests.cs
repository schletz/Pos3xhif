using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests for <see cref="LicensePlateValidator"/>.
/// </summary>
public class LicensePlateValidatorTests
{
    [Theory]
    [InlineData("W-12345A")]
    [InlineData("W-1234AB")]
    [InlineData("W-1000A")]
    [InlineData("G-123AB")]
    [InlineData("L-9999Z")]
    [InlineData("GU-123AB")]
    [InlineData("GU-12AB")]
    [InlineData("WZ-1BAA")]
    [InlineData("IL-11AAA")]
    [InlineData("MD-1234A")]
    [InlineData("KO-123AO")]
    public void IsValid_ValidPlate_ReturnsTrue(string plate)
    {
        Assert.True(LicensePlateValidator.IsValid(plate), $"'{plate}' should be valid.");
    }

    [Theory]
    [InlineData("W-123A", "one-letter district needs 5 or 6 characters")]
    [InlineData("GU-12345A", "two-letter district allows at most 5 characters")]
    [InlineData("GU-1AB", "two-letter district needs at least 4 characters")]
    [InlineData("W-01234A", "digit block starts with 0")]
    [InlineData("W-12345O", "letter block starts with O")]
    [InlineData("GU-123OA", "letter block starts with O")]
    [InlineData("W-12345Q", "letter Q")]
    [InlineData("GU-12QA", "letter Q")]
    [InlineData("W-12ABCD", "more than 3 letters")]
    [InlineData("W-12345", "no letter")]
    [InlineData("W-ABCDE", "no digit")]
    [InlineData("W-AB123", "letters before digits (personalised plate)")]
    [InlineData("W-1A2B3C", "digits and letters mixed")]
    [InlineData("GUX-123A", "district with 3 letters")]
    [InlineData("-12345A", "district missing")]
    [InlineData("w-12345a", "lower case")]
    [InlineData("W 12345A", "space instead of hyphen")]
    [InlineData("W12345A", "hyphen missing")]
    [InlineData("W--12345A", "two hyphens")]
    [InlineData("W-1234Ä", "umlaut")]
    [InlineData("W-12345A\n", "trailing line break")]
    [InlineData("", "empty string")]
    public void IsValid_InvalidPlate_ReturnsFalse(string plate, string reason)
    {
        Assert.False(LicensePlateValidator.IsValid(plate), $"'{plate}' should be invalid ({reason}).");
    }
}
