using Rechtschreibhilfe.Application;

namespace Rechtschreibhilfe.Tests;

/// <summary>
/// Tests <see cref="EditDistance.Naive"/>, <see cref="EditDistance.Table"/> and <see cref="EditDistance.Compute"/>.
/// </summary>
public class EditDistanceTests
{
    /// <summary>Known pairs: source, target, distance.</summary>
    public static TheoryData<string, string, int> KnownPairs => new()
    {
        { "", "", 0 },
        { "", "abc", 3 },          // insert three characters
        { "abc", "", 3 },          // delete three characters
        { "haus", "haus", 0 },
        { "haus", "maus", 1 },     // substitute h → m
        { "haus", "hau", 1 },      // delete s
        { "haus", "hausen", 2 },   // insert e and n
        { "tisch", "fisch", 1 },
        { "apfel", "äpfel", 1 },   // a and ä are different characters
        { "Haus", "haus", 1 },     // case sensitive
        { "kitten", "sitting", 3 },
        { "sonntag", "montag", 2 },
        { "straße", "strasse", 2 },
        { "fuß", "fuss", 2 },
        { "schule", "schüler", 2 },
        { "lehrer", "lehrerin", 2 },
        { "flasche", "tasche", 2 },
        { "stunde", "sekunde", 2 },
        { "ab", "ba", 2 },         // no swap operation: two substitutions
        { "abc", "cab", 2 },
        { "montag", "dienstag", 4 }
    };

    [Theory]
    [MemberData(nameof(KnownPairs))]
    public void NaiveComputesKnownDistances(string source, string target, int expected)
    {
        Assert.Equal(expected, EditDistance.Naive(source, target));
    }

    [Theory]
    [MemberData(nameof(KnownPairs))]
    public void ComputeReturnsKnownDistances(string source, string target, int expected)
    {
        Assert.Equal(expected, EditDistance.Compute(source, target));
    }

    [Theory]
    [MemberData(nameof(KnownPairs))]
    public void DistanceIsSymmetric(string source, string target, int expected)
    {
        Assert.Equal(expected, EditDistance.Compute(target, source));
    }

    [Fact]
    public void TableContainsAllPrefixDistances()
    {
        // Rows: "", t, to, tor. Columns: "", t, ti, tie, tier.
        int[,] expected =
        {
            { 0, 1, 2, 3, 4 },
            { 1, 0, 1, 2, 3 },
            { 2, 1, 1, 2, 3 },
            { 3, 2, 2, 2, 2 }
        };
        var table = EditDistance.Table("tor", "tier");
        Assert.Equal(4, table.GetLength(0));
        Assert.Equal(5, table.GetLength(1));
        Assert.Equal(expected, table);
    }

    [Fact]
    public void TableHasOneRowMoreThanSourceAndOneColumnMoreThanTarget()
    {
        var table = EditDistance.Table("sonntag", "montag");
        Assert.Equal(8, table.GetLength(0));
        Assert.Equal(7, table.GetLength(1));
        Assert.Equal(2, table[7, 6]);
    }

    [Fact]
    public void TableForEmptySourceIsASingleRow()
    {
        int[,] expected = { { 0, 1, 2, 3 } };
        Assert.Equal(expected, EditDistance.Table("", "abc"));
    }

    [Fact]
    public void NaiveAndComputeAgreeForShortWords()
    {
        string[] words =
        [
            "", "a", "ab", "ba", "abc", "aab", "baa", "aaa", "haus", "maus", "hase", "aus",
            "tor", "tür", "tier", "öl", "ohr", "fuß", "fluss", "regen", "segel", "abcdef", "fedcba"
        ];
        foreach (var source in words)
        {
            foreach (var target in words)
            {
                var naive = EditDistance.Naive(source, target);
                var dp = EditDistance.Compute(source, target);
                Assert.True(naive == dp, $"\"{source}\" → \"{target}\": Naive = {naive}, Compute = {dp}");
            }
        }
    }

    [Fact(Timeout = 10_000)]
    public async Task ComputeHandlesLongStrings()
    {
        // The plain recursion would never finish here. The table has 421 × 441 cells.
        var source = string.Concat(Enumerable.Repeat("donaudampfschifffahrt", 20));
        var target = string.Concat(Enumerable.Repeat("dampfschifffahrtsdonau", 20));
        var distance = await Task.Run(() => EditDistance.Compute(source, target));
        Assert.Equal(30, distance);
    }

    [Fact]
    public void NullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => EditDistance.Naive(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => EditDistance.Naive("a", null!));
        Assert.Throws<ArgumentNullException>(() => EditDistance.Table(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => EditDistance.Table("a", null!));
        Assert.Throws<ArgumentNullException>(() => EditDistance.Compute(null!, "a"));
        Assert.Throws<ArgumentNullException>(() => EditDistance.Compute("a", null!));
    }
}
