using Dfa.Application;

namespace Dfa.Tests;

/// <summary>
/// Tests the validation in the constructor of <see cref="Dfa{TState}"/>.
/// </summary>
public class DfaConstructorTests
{
    [Fact]
    public void ValidConfigurationDoesNotThrow()
    {
        var dfa = SampleDfa.Create();
        Assert.NotNull(dfa);
    }

    [Fact]
    public void EmptyTransitionTableIsAllowed()
    {
        var dfa = new Dfa<int>([0], "ab", new Dictionary<(int, char), int>(), 0, [0]);
        Assert.True(dfa.Accepts(""), "A DFA without transitions whose start state is final accepts ε.");
        Assert.False(dfa.Accepts("a"), "A DFA without transitions rejects every non-empty word.");
    }

    [Fact]
    public void EmptyFinalStatesAreAllowed()
    {
        var dfa = new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, []);
        Assert.False(dfa.Accepts("aa"), "Without final states no word is accepted.");
    }

    [Fact]
    public void NullStatesThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Dfa<int>(null!, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, SampleDfa.FinalStates));
        Assert.Equal("states", ex.ParamName);
    }

    [Fact]
    public void NullAlphabetThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Dfa<int>(SampleDfa.States, null!, SampleDfa.CreateTransitions(), 0, SampleDfa.FinalStates));
        Assert.Equal("alphabet", ex.ParamName);
    }

    [Fact]
    public void NullTransitionsThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, null!, 0, SampleDfa.FinalStates));
        Assert.Equal("transitions", ex.ParamName);
    }

    [Fact]
    public void NullStartThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Dfa<string>(["q0"], "a", new Dictionary<(string, char), string>(), null!, []));
        Assert.Equal("start", ex.ParamName);
    }

    [Fact]
    public void NullFinalStatesThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, null!));
        Assert.Equal("finalStates", ex.ParamName);
    }

    [Fact]
    public void DuplicateStateThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>([0, 1, 2, 1], SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, SampleDfa.FinalStates));
        Assert.Equal("states", ex.ParamName);
    }

    [Fact]
    public void DuplicateSymbolThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, "aba", SampleDfa.CreateTransitions(), 0, SampleDfa.FinalStates));
        Assert.Equal("alphabet", ex.ParamName);
    }

    [Fact]
    public void UnknownStartStateThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 7, SampleDfa.FinalStates));
        Assert.Equal("start", ex.ParamName);
    }

    [Fact]
    public void StartStateWithEmptyStatesThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>([], SampleDfa.Alphabet, new Dictionary<(int, char), int>(), 0, []));
        Assert.Equal("start", ex.ParamName);
    }

    [Fact]
    public void UnknownFinalStateThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, [2, 9]));
        Assert.Equal("finalStates", ex.ParamName);
    }

    [Fact]
    public void DuplicateFinalStateThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, SampleDfa.CreateTransitions(), 0, [2, 2]));
        Assert.Equal("finalStates", ex.ParamName);
    }

    [Fact]
    public void TransitionFromUnknownStateThrowsArgumentException()
    {
        var transitions = SampleDfa.CreateTransitions();
        transitions[(5, 'a')] = 1;
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, transitions, 0, SampleDfa.FinalStates));
        Assert.Equal("transitions", ex.ParamName);
    }

    [Fact]
    public void TransitionToUnknownStateThrowsArgumentException()
    {
        var transitions = SampleDfa.CreateTransitions();
        transitions[(2, 'a')] = 5;
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, transitions, 0, SampleDfa.FinalStates));
        Assert.Equal("transitions", ex.ParamName);
    }

    [Fact]
    public void TransitionWithUnknownSymbolThrowsArgumentException()
    {
        var transitions = SampleDfa.CreateTransitions();
        transitions[(2, 'c')] = 0;
        var ex = Assert.Throws<ArgumentException>(() =>
            new Dfa<int>(SampleDfa.States, SampleDfa.Alphabet, transitions, 0, SampleDfa.FinalStates));
        Assert.Equal("transitions", ex.ParamName);
    }

    [Fact]
    public void LaterChangesOfTheArgumentsHaveNoEffect()
    {
        var states = new List<int>(SampleDfa.States);
        var transitions = SampleDfa.CreateTransitions();
        var finalStates = new List<int>(SampleDfa.FinalStates);
        var dfa = new Dfa<int>(states, SampleDfa.Alphabet, transitions, 0, finalStates);

        transitions[(2, 'a')] = 2;
        transitions.Remove((1, 'b'));
        finalStates.Add(1);

        Assert.True(dfa.Accepts("aba"), "Removing a transition from the caller's dictionary must not change the DFA.");
        Assert.False(dfa.Accepts("aaa"), "Adding a transition to the caller's dictionary must not change the DFA.");
        Assert.False(dfa.Accepts("a"), "Adding a final state to the caller's list must not change the DFA.");
    }
}
