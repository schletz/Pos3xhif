using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests for <see cref="IbanValidator"/>. The checksum tests belong to the optional extra task.
/// </summary>
public class IbanValidatorTests
{
    [Theory]
    [InlineData("AT611904300234573201")]
    [InlineData("AT61 1904 3002 3457 3201")]
    [InlineData("AT00 0000 0000 0000 0000")]
    public void IsValidFormat_ValidFormat_ReturnsTrue(string iban)
    {
        Assert.True(IbanValidator.IsValidFormat(iban), $"'{iban}' should have a valid format.");
    }

    [Theory]
    [InlineData("at611904300234573201", "lower case country code")]
    [InlineData("DE89370400440532013000", "German IBAN")]
    [InlineData("AT61190430023457320", "19 characters")]
    [InlineData("AT6119043002345732011", "21 characters")]
    [InlineData("AT61190430023457320A", "letter in the account number")]
    [InlineData("AT61 19043002 3457 3201", "groups only partly separated")]
    [InlineData("AT61  1904 3002 3457 3201", "two spaces")]
    [InlineData("AT61-1904-3002-3457-3201", "hyphens instead of spaces")]
    [InlineData("AT 6119 0430 0234 5732 01", "wrong grouping")]
    [InlineData(" AT611904300234573201", "leading space")]
    [InlineData("AT611904300234573201 ", "trailing space")]
    [InlineData("AT611904300234573201\n", "trailing line break")]
    [InlineData("", "empty string")]
    public void IsValidFormat_InvalidFormat_ReturnsFalse(string iban, string reason)
    {
        Assert.False(IbanValidator.IsValidFormat(iban), $"'{iban}' should have an invalid format ({reason}).");
    }

    [Theory]
    [InlineData("AT611904300234573201")]
    [InlineData("AT61 1904 3002 3457 3201")]
    [InlineData("AT782011182122344400")]
    [InlineData("AT48 3200 0000 1234 5864")]
    [InlineData("AT871200052066543301")]
    [InlineData("AT96 6000 0000 0012 3456")]
    [InlineData("AT271400004011234567")]
    public void HasValidChecksum_CorrectCheckDigits_ReturnsTrue(string iban)
    {
        Assert.True(IbanValidator.HasValidChecksum(iban), $"'{iban}' should have valid check digits.");
    }

    [Theory]
    [InlineData("AT621904300234573201", "check digits changed")]
    [InlineData("AT611904300234573202", "last digit changed")]
    [InlineData("AT611904300234573210", "two digits swapped")]
    [InlineData("AT00 0000 0000 0000 0000", "check digits 00")]
    [InlineData("AT61 1904 3002 3457 320", "invalid format")]
    public void HasValidChecksum_WrongIban_ReturnsFalse(string iban, string reason)
    {
        Assert.False(IbanValidator.HasValidChecksum(iban), $"'{iban}' should be rejected ({reason}).");
    }
}
