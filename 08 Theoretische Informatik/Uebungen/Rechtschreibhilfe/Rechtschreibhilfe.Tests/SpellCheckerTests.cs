using Rechtschreibhilfe.Application;

namespace Rechtschreibhilfe.Tests;

/// <summary>
/// Tests <see cref="SpellChecker"/> with a small word list, so the expected results can be checked by hand.
/// </summary>
public class SpellCheckerTests
{
    private static readonly string[] SmallList =
    [
        "haus", "maus", "laus", "hase", "hose", "aus", "raus", "hausen",
        "tor", "tür", "tier", "apfel", "äpfel", "haus"
    ];

    private readonly SpellChecker _checker = new(SmallList);

    [Fact]
    public void CountIgnoresDuplicates()
    {
        Assert.Equal(13, _checker.Count);
    }

    [Theory]
    [InlineData("haus", true)]
    [InlineData("äpfel", true)]
    [InlineData("hau", false)]
    [InlineData("Haus", false)]   // case sensitive
    [InlineData("", false)]
    public void ContainsComparesExactly(string word, bool expected)
    {
        Assert.Equal(expected, _checker.Contains(word));
    }

    [Fact]
    public void LaterChangesOfTheListDoNotChangeTheChecker()
    {
        var words = new List<string> { "haus" };
        var checker = new SpellChecker(words);
        words.Add("maus");
        Assert.False(checker.Contains("maus"));
        Assert.Equal(1, checker.Count);
    }

    [Fact]
    public void SuggestSortsEqualDistancesAlphabetically()
    {
        Assert.Equal(
            [new("aus", 1), new("haus", 1), new("laus", 1), new("maus", 1), new("raus", 1)],
            _checker.Suggest("xaus", 1, 10));
    }

    [Fact]
    public void SuggestReturnsAtMostCountEntries()
    {
        Assert.Equal(
            [new Suggestion("aus", 1), new("haus", 1), new("laus", 1)],
            _checker.Suggest("xaus", 1, 3));
    }

    [Fact]
    public void SuggestSortsByDistanceFirst()
    {
        Assert.Equal(
            [
                new Suggestion("haus", 0), new("aus", 1), new("laus", 1), new("maus", 1), new("raus", 1),
                new("hase", 2), new("hausen", 2)
            ],
            _checker.Suggest("haus", 2, 10));
    }

    [Fact]
    public void SuggestCutsOffAfterSortingByDistance()
    {
        Assert.Equal(
            [new Suggestion("haus", 0), new("aus", 1), new("laus", 1), new("maus", 1)],
            _checker.Suggest("haus", 2, 4));
    }

    [Fact]
    public void SuggestWithMaxDistanceZeroFindsOnlyTheWordItself()
    {
        Assert.Equal([new Suggestion("haus", 0)], _checker.Suggest("haus", 0, 10));
        Assert.Empty(_checker.Suggest("baum", 0, 10));
    }

    [Fact]
    public void SuggestComparesUmlautsByCharacterCode()
    {
        // Ordinal order: 'a' (U+0061) < 'o' (U+006F) < 'ä' (U+00E4) < 'ü' (U+00FC).
        Assert.Equal([new Suggestion("apfel", 1), new("äpfel", 1)], _checker.Suggest("bpfel", 1, 10));
        Assert.Equal([new Suggestion("tier", 1), new("tor", 1), new("tür", 1)], _checker.Suggest("tir", 1, 10));
    }

    [Fact]
    public void SuggestWithCountZeroIsEmpty()
    {
        Assert.Empty(_checker.Suggest("haus", 2, 0));
    }

    [Fact]
    public void SuggestForTheEmptyWord()
    {
        Assert.Equal(
            [new Suggestion("aus", 3), new("tor", 3), new("tür", 3)],
            _checker.Suggest("", 3, 10));
    }

    [Fact]
    public void SuggestRejectsInvalidArguments()
    {
        Assert.Throws<ArgumentNullException>(() => _checker.Suggest(null!, 1, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => _checker.Suggest("haus", -1, 10));
        Assert.Throws<ArgumentOutOfRangeException>(() => _checker.Suggest("haus", 1, -1));
    }

    [Fact]
    public void ConstructorRejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new SpellChecker(null!));
        Assert.ThrowsAny<ArgumentException>(() => new SpellChecker(["haus", null!]));
    }

    [Fact]
    public void ContainsNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _checker.Contains(null!));
    }
}
