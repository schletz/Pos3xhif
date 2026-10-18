using Dfa.Application;

namespace Dfa.Tests;

/// <summary>
/// Tests <see cref="Dfa{TState}.Accepts"/> and <see cref="Dfa{TState}.Trace"/> with the fixture a b* a.
/// </summary>
public class DfaRunTests
{
    private readonly Dfa<int> _dfa = SampleDfa.Create();

    [Theory]
    [InlineData("aa")]
    [InlineData("aba")]
    [InlineData("abbbba")]
    public void AcceptsWordsOfTheLanguage(string word)
    {
        Assert.True(_dfa.Accepts(word), $"'{word}' is in a b* a and must be accepted.");
    }

    [Theory]
    [InlineData("")]           // ends in the start state, which is not final
    [InlineData("a")]          // ends in state 1, which is not final
    [InlineData("abb")]        // ends in state 1
    [InlineData("b")]          // δ(0, b) is missing
    [InlineData("aaa")]        // δ(2, a) is missing after a final state
    [InlineData("aab")]        // δ(2, b) is missing after a final state
    [InlineData("aca")]        // c is not in Σ
    [InlineData("AA")]         // symbols are case sensitive
    public void RejectsWordsOutsideTheLanguage(string word)
    {
        Assert.False(_dfa.Accepts(word), $"'{word}' is not in a b* a and must be rejected.");
    }

    [Fact]
    public void AcceptsNullThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _dfa.Accepts(null!));
        Assert.Equal("word", ex.ParamName);
    }

    [Fact]
    public void TraceOfEmptyWordContainsOnlyTheStartState()
    {
        Assert.Equal([0], _dfa.Trace(""));
    }

    [Fact]
    public void TraceContainsStartStateAndOneStatePerCharacter()
    {
        Assert.Equal([0, 1, 1, 1, 2], _dfa.Trace("abba"));
    }

    [Fact]
    public void TraceOfRejectedCompleteWordContainsAllStates()
    {
        Assert.Equal([0, 1, 1], _dfa.Trace("ab"));
    }

    [Fact]
    public void TraceStopsAtFirstCharacterWithoutTransition()
    {
        Assert.Equal([0], _dfa.Trace("ba"));
    }

    [Fact]
    public void TraceStopsInTheStateWhereTheTransitionIsMissing()
    {
        Assert.Equal([0, 1, 2], _dfa.Trace("aaab"));
    }

    [Fact]
    public void TraceStopsAtSymbolOutsideTheAlphabet()
    {
        Assert.Equal([0, 1, 1], _dfa.Trace("abxa"));
    }

    [Fact]
    public void TraceNullThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _dfa.Trace(null!));
        Assert.Equal("word", ex.ParamName);
    }

    [Fact]
    public void WorksWithStringStates()
    {
        var dfa = new Dfa<string>(
            ["even", "odd"], "1",
            new Dictionary<(string, char), string> { [("even", '1')] = "odd", [("odd", '1')] = "even" },
            "even", ["even"]);
        Assert.True(dfa.Accepts("1111"), "An even number of ones must be accepted.");
        Assert.False(dfa.Accepts("111"), "An odd number of ones must be rejected.");
        Assert.Equal(["even", "odd", "even"], dfa.Trace("11"));
    }
}
