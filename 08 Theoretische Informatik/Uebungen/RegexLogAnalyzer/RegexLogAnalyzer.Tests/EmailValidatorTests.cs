using RegexLogAnalyzer.Application;

namespace RegexLogAnalyzer.Tests;

/// <summary>
/// Tests for <see cref="EmailValidator"/>.
/// </summary>
public class EmailValidatorTests
{
    [Theory]
    [InlineData("max.mustermann@spengergasse.at")]
    [InlineData("a@b.co")]
    [InlineData("first_last+tag@mail.example.com")]
    [InlineData("UPPER.CASE@EXAMPLE.AT")]
    [InlineData("x-y@sub-domain.example.org")]
    [InlineData("user%test@example.com")]
    [InlineData("1234@123.example.com")]
    public void IsValid_ValidAddress_ReturnsTrue(string address)
    {
        Assert.True(EmailValidator.IsValid(address), $"'{address}' should be valid.");
    }

    [Theory]
    [InlineData("plainaddress", "no @")]
    [InlineData("@example.com", "local part missing")]
    [InlineData("max@", "domain missing")]
    [InlineData("max@@example.com", "two @ in a row")]
    [InlineData("max@sub.example.com@x.at", "two @")]
    [InlineData("max@example", "only one domain label")]
    [InlineData(".max@example.com", "local part starts with a dot")]
    [InlineData("max.@example.com", "local part ends with a dot")]
    [InlineData("max..m@example.com", "two dots in the local part")]
    [InlineData("max@-example.com", "label starts with a hyphen")]
    [InlineData("max@example-.com", "label ends with a hyphen")]
    [InlineData("max@example..com", "empty label")]
    [InlineData("max@.example.com", "domain starts with a dot")]
    [InlineData("max@example.com.", "domain ends with a dot")]
    [InlineData("max@example.c", "top-level domain with one letter")]
    [InlineData("max@example.c0m", "digit in the top-level domain")]
    [InlineData("max@exa_mple.com", "underscore in the domain")]
    [InlineData("max mustermann@example.com", "space")]
    [InlineData("müller@example.at", "umlaut")]
    [InlineData("max@example.com\n", "trailing line break")]
    public void IsValid_InvalidAddress_ReturnsFalse(string address, string reason)
    {
        Assert.False(EmailValidator.IsValid(address), $"'{address}' should be invalid ({reason}).");
    }
}
