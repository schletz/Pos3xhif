using Dfa.Application;
using static Dfa.Application.DivisibleByThreeState;

namespace Dfa.Tests;

/// <summary>
/// Tests the automaton for binary numbers divisible by 3.
/// </summary>
public class DivisibleByThreeTests
{
    private readonly Dfa<DivisibleByThreeState> _dfa = DivisibleByThree.Create();

    [Theory]
    [InlineData("0")]
    [InlineData("11")]          // 3
    [InlineData("110")]         // 6
    [InlineData("1001")]        // 9
    [InlineData("1111")]        // 15
    [InlineData("000")]         // 0 with leading zeros
    [InlineData("0011")]        // 3 with leading zeros
    [InlineData("1100110")]     // 102
    public void AcceptsNumbersDivisibleByThree(string word)
    {
        Assert.True(_dfa.Accepts(word), $"{word} = {Convert.ToInt32(word, 2)} is divisible by 3.");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("10")]
    [InlineData("100")]
    [InlineData("101")]
    [InlineData("0010")]
    public void RejectsNumbersNotDivisibleByThree(string word)
    {
        Assert.False(_dfa.Accepts(word), $"{word} = {Convert.ToInt32(word, 2)} is not divisible by 3.");
    }

    [Theory]
    [InlineData("")]            // ε is not a number
    [InlineData("12")]
    [InlineData("1 1")]
    [InlineData("-11")]
    [InlineData("0b11")]
    public void RejectsWordsThatAreNoBinaryNumbers(string word)
    {
        Assert.False(_dfa.Accepts(word), $"'{word}' is not a binary number.");
    }

    [Fact]
    public void AgreesWithModuloForAllNumbersUpTo1024()
    {
        for (var n = 0; n <= 1024; n++)
        {
            var word = Convert.ToString(n, 2);
            Assert.True(_dfa.Accepts(word) == (n % 3 == 0),
                $"Accepts(\"{word}\") must be {n % 3 == 0} because {n} mod 3 = {n % 3}.");
        }
    }

    [Fact]
    public void TraceFollowsTheRemainders()
    {
        // 1 → 1, 11 → 3, 110 → 6, 1101 → 13
        Assert.Equal([Start, Remainder1, Remainder0, Remainder0, Remainder1], _dfa.Trace("1101"));
    }

    [Fact]
    public void TraceOfLeadingZeroStartsWithRemainder0()
    {
        Assert.Equal([Start, Remainder0, Remainder1, Remainder2], _dfa.Trace("010"));
    }
}
