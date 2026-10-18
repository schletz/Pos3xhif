using Routenplaner.Application;

namespace Routenplaner.Tests;

/// <summary>
/// Tests <see cref="ConnectionReader.Parse"/> with small inputs.
/// </summary>
public class ConnectionReaderTests
{
    private const string Header = "Linie;Von;Nach;Minuten";

    [Fact]
    public void Parse_DataLines_ReturnsConnectionsInOrder()
    {
        var result = ConnectionReader.Parse([Header, "U1;Karlsplatz;Stephansplatz;2", "U3;Stephansplatz;Stubentor;2"]);

        Assert.Equal(
            [new Connection("U1", "Karlsplatz", "Stephansplatz", 2), new Connection("U3", "Stephansplatz", "Stubentor", 2)],
            result);
    }

    [Fact]
    public void Parse_CommentsAndEmptyLines_AreSkipped()
    {
        var result = ConnectionReader.Parse(["# Netz", "", Header, "   ", "# U1", "U1;Karlsplatz;Stephansplatz;2", ""]);

        Assert.Equal([new Connection("U1", "Karlsplatz", "Stephansplatz", 2)], result);
    }

    [Fact]
    public void Parse_SpacesAroundFields_AreRemoved()
    {
        var result = ConnectionReader.Parse([" Linie;Von;Nach;Minuten ", " U4 ; Ober St. Veit ;Hietzing; 3 "]);

        Assert.Equal([new Connection("U4", "Ober St. Veit", "Hietzing", 3)], result);
    }

    [Fact]
    public void Parse_HeaderOnly_ReturnsEmptyList()
    {
        Assert.Empty(ConnectionReader.Parse(["# nothing yet", Header]));
    }

    [Fact]
    public void Parse_NoHeaderAndNoData_ReturnsEmptyList()
    {
        Assert.Empty(ConnectionReader.Parse([]));
        Assert.Empty(ConnectionReader.Parse(["# only a comment", ""]));
    }

    [Theory]
    [InlineData("U1;Karlsplatz;Stephansplatz")]          // 3 fields
    [InlineData("U1;Karlsplatz;Stephansplatz;2;x")]      // 5 fields
    [InlineData(";Karlsplatz;Stephansplatz;2")]          // empty line name
    [InlineData("U1;  ;Stephansplatz;2")]                // empty station
    [InlineData("U1;Karlsplatz;;2")]                     // empty station
    [InlineData("U1;Karlsplatz;Stephansplatz;")]         // empty minutes
    [InlineData("U1;Karlsplatz;Stephansplatz;zwei")]     // not a number
    [InlineData("U1;Karlsplatz;Stephansplatz;1.5")]      // not an integer
    [InlineData("U1;Karlsplatz;Stephansplatz;0")]        // not positive
    [InlineData("U1;Karlsplatz;Stephansplatz;-2")]       // not positive
    [InlineData("U1;Karlsplatz;Karlsplatz;2")]           // both stations are equal
    [InlineData("U1,Karlsplatz,Stephansplatz,2")]        // wrong separator
    public void Parse_InvalidDataLine_ReportsItsLineNumber(string line)
    {
        // The invalid line is line 4; comment lines count.
        string[] lines = ["# Netz", Header, "U1;Taubstummengasse;Karlsplatz;1", line, "U1;Stephansplatz;Schwedenplatz;1"];

        var ex = Assert.Throws<CsvFormatException>(() => ConnectionReader.Parse(lines));

        Assert.True(ex.LineNumber == 4, $"'{line}' is invalid and in line 4, but LineNumber is {ex.LineNumber}.");
    }

    [Theory]
    [InlineData("Linie,Von,Nach,Minuten")]
    [InlineData("linie;von;nach;minuten")]
    [InlineData("Linie;Von;Nach")]
    [InlineData("U1;Karlsplatz;Stephansplatz;2")]
    public void Parse_FirstLineIsNotTheHeader_ReportsItsLineNumber(string first)
    {
        var ex = Assert.Throws<CsvFormatException>(() => ConnectionReader.Parse(["# Netz", "", first, "U1;Karlsplatz;Stephansplatz;2"]));

        Assert.True(ex.LineNumber == 3, $"'{first}' is not the header and in line 3, but LineNumber is {ex.LineNumber}.");
    }

    [Fact]
    public void Parse_HeaderTwice_ReportsTheSecondOne()
    {
        var ex = Assert.Throws<CsvFormatException>(() => ConnectionReader.Parse([Header, "U1;Karlsplatz;Stephansplatz;2", Header]));

        Assert.Equal(3, ex.LineNumber);
    }

    [Fact]
    public void Parse_Null_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => ConnectionReader.Parse(null!));
        Assert.Equal("lines", ex.ParamName);
    }

    [Fact]
    public void CsvFormatException_IsAFormatException()
    {
        var ex = new CsvFormatException("Minutes are missing.", 7);

        Assert.IsAssignableFrom<FormatException>(ex);
        Assert.Equal(7, ex.LineNumber);
        Assert.Contains("7", ex.Message);
    }
}
