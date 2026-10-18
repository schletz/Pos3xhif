using SudokuSolver.Application;

namespace SudokuSolver.Tests;

/// <summary>
/// Tests <see cref="SudokuBoard.Parse"/>, the indexer, <see cref="SudokuBoard.ToString"/> and <see cref="SudokuBoard.Clone"/>.
/// </summary>
public class SudokuBoardTests
{
    [Fact]
    public void ParseReadsTheCellsRowByRow()
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        Assert.Equal(5, board[0, 0]);
        Assert.Equal(3, board[0, 1]);
        Assert.Equal(SudokuBoard.Empty, board[0, 2]);
        Assert.Equal(7, board[0, 4]);
        Assert.Equal(6, board[1, 0]);
        Assert.Equal(1, board[1, 3]);
        Assert.Equal(8, board[8, 4]);
        Assert.Equal(9, board[8, 8]);
    }

    [Fact]
    public void ToStringReturns81CharactersWithDotsForEmptyCells()
    {
        Assert.Equal(Puzzles.Wikipedia, SudokuBoard.Parse(Puzzles.Wikipedia).ToString());
    }

    [Fact]
    public void ParseAcceptsZeroForEmptyCells()
    {
        var withZeros = Puzzles.Wikipedia.Replace('.', '0');
        var board = SudokuBoard.Parse(withZeros);
        Assert.Equal(SudokuBoard.Empty, board[0, 2]);
        Assert.Equal(Puzzles.Wikipedia, board.ToString());
    }

    [Fact]
    public void ParseIgnoresWhitespace()
    {
        var text = """
            53. .7. ...
            6.. 195 ...
            .98 ... .6.
            8.. .6. ..3
            4.. 8.3 ..1
            7.. .2. ..6
            .6. ... 28.
            ... 419 ..5
            ... .8. .79
            """;
        Assert.Equal(Puzzles.Wikipedia, SudokuBoard.Parse(text).ToString());
    }

    [Fact]
    public void ParseDoesNotCheckTheRules()
    {
        var board = SudokuBoard.Parse("55" + new string('.', 79));
        Assert.Equal(5, board[0, 1]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    public void ParseRejectsTooFewCells(string text)
    {
        Assert.Throws<FormatException>(() => SudokuBoard.Parse(text));
    }

    [Fact]
    public void ParseRejects80Cells()
    {
        Assert.Throws<FormatException>(() => SudokuBoard.Parse(Puzzles.Wikipedia[..80]));
    }

    [Fact]
    public void ParseRejects82Cells()
    {
        Assert.Throws<FormatException>(() => SudokuBoard.Parse(Puzzles.Wikipedia + "."));
    }

    [Theory]
    [InlineData('x')]
    [InlineData('-')]
    [InlineData('*')]
    [InlineData('A')]
    [InlineData('١')]   // Arabic-Indic digit one: a digit for char.IsDigit, but not allowed here
    public void ParseRejectsInvalidCharacters(char invalid)
    {
        var text = invalid + Puzzles.Wikipedia[1..];
        Assert.Throws<FormatException>(() => SudokuBoard.Parse(text));
    }

    [Fact]
    public void ParseNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => SudokuBoard.Parse(null!));
    }

    [Fact]
    public void IndexerSetsAndClearsACell()
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        board[0, 2] = 4;
        Assert.Equal(4, board[0, 2]);
        board[0, 2] = SudokuBoard.Empty;
        Assert.Equal(Puzzles.Wikipedia, board.ToString());
    }

    [Theory]
    [InlineData(-1, 0)]
    [InlineData(9, 0)]
    [InlineData(0, -1)]
    [InlineData(0, 9)]
    public void IndexerRejectsPositionsOutsideTheBoard(int row, int column)
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        Assert.Throws<ArgumentOutOfRangeException>(() => board[row, column]);
        Assert.Throws<ArgumentOutOfRangeException>(() => board[row, column] = 1);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(10)]
    public void IndexerRejectsValuesOutsideZeroToNine(int value)
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        Assert.Throws<ArgumentOutOfRangeException>(() => board[0, 2] = value);
    }

    [Fact]
    public void CloneIsIndependentOfTheOriginal()
    {
        var original = SudokuBoard.Parse(Puzzles.Wikipedia);
        var copy = original.Clone();
        copy[0, 2] = 4;
        original[0, 3] = 6;
        Assert.Equal(SudokuBoard.Empty, original[0, 2]);
        Assert.Equal(SudokuBoard.Empty, copy[0, 3]);
        Assert.Equal(4, copy[0, 2]);
    }
}
