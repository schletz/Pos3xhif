using System.Text.RegularExpressions;
using Rechtschreibhilfe.Application;

namespace Rechtschreibhilfe.Tests;

/// <summary>
/// Tests the German word list and suggestions based on it. The expected suggestions only hold
/// as long as <c>Data/woerter.txt</c> is not changed.
/// </summary>
public class GermanWordListTests
{
    private static readonly IReadOnlyList<string> Words = WordList.LoadGerman();

    private static SpellChecker CreateChecker() => new(Words);

    [Fact]
    public void WordListHasBetween1000And3000Words()
    {
        Assert.InRange(Words.Count, 1000, 3000);
    }

    [Fact]
    public void WordListContainsOnlyLowercaseWords()
    {
        var invalid = Words.Where(w => !Regex.IsMatch(w, "^[a-zäöüß]+$")).ToList();
        Assert.True(invalid.Count == 0, $"Invalid words: {string.Join(", ", invalid)}");
    }

    [Fact]
    public void WordListIsSortedOrdinalWithoutDuplicates()
    {
        for (var i = 1; i < Words.Count; i++)
        {
            Assert.True(string.CompareOrdinal(Words[i - 1], Words[i]) < 0,
                $"\"{Words[i - 1]}\" must come before \"{Words[i]}\".");
        }
    }

    [Theory]
    [InlineData("straße")]
    [InlineData("über")]
    [InlineData("mädchen")]
    [InlineData("jause")]
    public void WordListContainsUmlautsAndSharpS(string word)
    {
        Assert.Contains(word, Words);
    }

    [Fact]
    public void CheckerContainsEveryWordOfTheList()
    {
        Assert.Equal(Words.Count, CreateChecker().Count);
    }

    [Fact]
    public void SuggestHaus()
    {
        Assert.Equal(
            [new Suggestion("haus", 0), new("aus", 1), new("hals", 1), new("haut", 1), new("maus", 1)],
            CreateChecker().Suggest("haus", 1, 10));
    }

    [Fact]
    public void SuggestStrase()
    {
        Assert.Equal(
            [new Suggestion("strafe", 1), new("straße", 1), new("strahl", 2), new("strand", 2)],
            CreateChecker().Suggest("strase", 2, 5));
    }

    [Fact]
    public void SuggestFahrad()
    {
        Assert.Equal(
            [new Suggestion("fahrrad", 1), new("fahren", 2), new("fahrer", 2), new("fahrt", 2)],
            CreateChecker().Suggest("fahrad", 2, 5));
    }

    [Fact]
    public void SuggestSchuhle()
    {
        Assert.Equal(
            [new Suggestion("schuhe", 1), new("schule", 1), new("schale", 2), new("schuh", 2), new("schuld", 2)],
            CreateChecker().Suggest("schuhle", 2, 5));
    }

    [Fact]
    public void SuggestLererIsCutOffAfterFiveEntries()
    {
        // 30 words have a distance of at most 2, only the first five are returned.
        Assert.Equal(
            [new Suggestion("leder", 1), new("leer", 1), new("lehrer", 1), new("leser", 1), new("eher", 2)],
            CreateChecker().Suggest("lerer", 2, 5));
    }

    [Fact]
    public void SuggestUberSortsUmlautsByCharacterCode()
    {
        Assert.Equal(
            [new Suggestion("aber", 1), new("ufer", 1), new("über", 1)],
            CreateChecker().Suggest("uber", 1, 10));
    }

    [Fact]
    public void SuggestBiebliothek()
    {
        Assert.Equal([new Suggestion("bibliothek", 1)], CreateChecker().Suggest("biebliothek", 2, 5));
    }

    [Fact]
    public void SuggestFindsNothingForNonsense()
    {
        Assert.Empty(CreateChecker().Suggest("xyzxyz", 2, 5));
    }
}
