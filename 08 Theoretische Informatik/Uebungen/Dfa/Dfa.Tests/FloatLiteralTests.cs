using Dfa.Application;

namespace Dfa.Tests;

/// <summary>
/// Tests the automaton for float literals (rule: see README).
/// </summary>
public class FloatLiteralTests
{
    private readonly Dfa<FloatLiteralState> _dfa = FloatLiteral.Create();

    [Theory]
    [InlineData("3.14")]
    [InlineData("-0.5")]
    [InlineData("+2.0")]
    [InlineData("1e10")]
    [InlineData("2.5E-3")]
    [InlineData("6.022e+23")]
    [InlineData("0.0")]
    [InlineData("007.50")]      // leading and trailing zeros are allowed
    [InlineData("1E0")]
    [InlineData("-1e-007")]
    [InlineData("123456789.0123456789")]
    public void AcceptsFloatLiterals(string word)
    {
        Assert.True(_dfa.Accepts(word), $"'{word}' is a float literal and must be accepted.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("42")]          // integer without fraction or exponent
    [InlineData("-7")]
    [InlineData(".5")]          // at least one digit before the point
    [InlineData("5.")]          // at least one digit after the point
    [InlineData("1.e5")]
    [InlineData("1e")]          // exponent without digits
    [InlineData("1e+")]
    [InlineData("e5")]
    [InlineData("+")]
    [InlineData("+.5")]
    [InlineData("--1.0")]
    [InlineData("+-1.0")]
    [InlineData("1.0.0")]
    [InlineData("1e5.0")]       // the exponent is an integer
    [InlineData("1e5e5")]
    [InlineData("1,5")]         // decimal comma is not allowed
    [InlineData("1.5f")]        // no type suffix
    [InlineData("1_000.0")]     // no digit separator
    [InlineData(" 1.0")]
    [InlineData("1.0 ")]
    public void RejectsInvalidLiterals(string word)
    {
        Assert.False(_dfa.Accepts(word), $"'{word}' is not a float literal and must be rejected.");
    }
}
