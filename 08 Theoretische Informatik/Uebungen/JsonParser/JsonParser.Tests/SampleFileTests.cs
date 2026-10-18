using System.Text.Json;
using JsonParser.Application;

namespace JsonParser.Tests;

/// <summary>
/// Parses the sample files in <c>data/</c> and compares the result with System.Text.Json.
/// The files are copied to the output directory (see the csproj).
/// </summary>
public class SampleFileTests
{
    private static string DataPath(string folder, string fileName = "") =>
        Path.Combine(AppContext.BaseDirectory, "data", folder, fileName);

    public static TheoryData<string> ValidFiles()
    {
        var data = new TheoryData<string>();
        foreach (var file in Directory.GetFiles(DataPath("valid"), "*.json").Order())
        {
            data.Add(Path.GetFileName(file));
        }
        return data;
    }

    [Fact]
    public void DataFolder_ContainsSampleFiles()
    {
        Assert.True(ValidFiles().Count >= 8, $"Expected at least 8 sample files in {DataPath("valid")}.");
    }

    [Theory]
    [MemberData(nameof(ValidFiles))]
    public void Parse_ValidFile_MatchesSystemTextJson(string fileName)
    {
        var json = File.ReadAllText(DataPath("valid", fileName));
        using var expected = JsonDocument.Parse(json);

        var actual = Parser.Parse(json);

        JsonAssert.Equivalent(expected.RootElement, actual);
    }

    [Theory]
    [MemberData(nameof(ValidFiles))]
    public void Parse_CompactOutputOfValidFile_ReturnsEqualValue(string fileName)
    {
        // ToString() writes compact JSON, parsing it again must give the same tree.
        var value = Parser.Parse(File.ReadAllText(DataPath("valid", fileName)));
        Assert.Equal(value, Parser.Parse(value.ToString()));
    }

    // Only line and column are checked: git may convert the line breaks of the files to \r\n,
    // which changes the position but not line and column.
    [Theory]
    [InlineData("trailing_comma.json", 3, 21)]
    [InlineData("fehlender_doppelpunkt.json", 3, 14)]
    [InlineData("unbekanntes_escape.json", 3, 10)]
    public void Parse_InvalidFile_ReportsLineAndColumn(string fileName, int line, int column)
    {
        var json = File.ReadAllText(DataPath("invalid", fileName));
        var ex = Assert.Throws<JsonParseException>(() => Parser.Parse(json));
        Assert.Equal((line, column), (ex.Line, ex.Column));
    }
}
