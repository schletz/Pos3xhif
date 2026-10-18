namespace Dfa.Application;

/// <summary>
/// States of the automaton that recognizes binary numbers divisible by 3.
/// </summary>
public enum DivisibleByThreeState
{
    /// <summary>No digit has been read yet.</summary>
    Start,

    /// <summary>The digits read so far form a number n with n mod 3 = 0.</summary>
    Remainder0,

    /// <summary>The digits read so far form a number n with n mod 3 = 1.</summary>
    Remainder1,

    /// <summary>The digits read so far form a number n with n mod 3 = 2.</summary>
    Remainder2
}
