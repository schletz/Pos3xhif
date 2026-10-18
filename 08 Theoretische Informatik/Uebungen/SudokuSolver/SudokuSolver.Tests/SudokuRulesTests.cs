using SudokuSolver.Application;

namespace SudokuSolver.Tests;

/// <summary>
/// Tests the rule checks <see cref="SudokuBoard.CanPlace"/>, <see cref="SudokuBoard.Candidates"/>,
/// <see cref="SudokuBoard.IsValid"/> and <see cref="SudokuBoard.IsComplete"/>.
/// </summary>
public class SudokuRulesTests
{
    private readonly SudokuBoard _wikipedia = SudokuBoard.Parse(Puzzles.Wikipedia);

    [Theory]
    [InlineData(2, true)]
    [InlineData(5, true)]
    [InlineData(1, false)]     // row 4 contains a 1
    [InlineData(9, false)]     // column 1 contains a 9
    [InlineData(7, false)]     // the box in the middle left contains a 7
    public void CanPlaceChecksRowColumnAndBox(int digit, bool expected)
    {
        Assert.Equal(expected, _wikipedia.CanPlace(4, 1, digit));
    }

    [Fact]
    public void CanPlaceIgnoresTheCellItself()
    {
        Assert.True(_wikipedia.CanPlace(0, 0, 5), "The 5 in row 0, column 0 must not conflict with itself.");
        Assert.False(_wikipedia.CanPlace(0, 0, 3), "Row 0 already contains a 3.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void CanPlaceRejectsInvalidDigits(int digit)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _wikipedia.CanPlace(0, 2, digit));
    }

    [Fact]
    public void CanPlaceRejectsPositionsOutsideTheBoard()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _wikipedia.CanPlace(9, 0, 1));
    }

    [Fact]
    public void CandidatesAreSortedAscending()
    {
        Assert.Equal([1, 2, 4], _wikipedia.Candidates(0, 2));
        Assert.Equal([2, 5], _wikipedia.Candidates(4, 1));
    }

    [Fact]
    public void CandidatesOfAFilledCellAreEmpty()
    {
        Assert.Empty(_wikipedia.Candidates(0, 0));
    }

    [Fact]
    public void CandidatesOfAnEmptyBoardAreAllDigits()
    {
        Assert.Equal([1, 2, 3, 4, 5, 6, 7, 8, 9], SudokuBoard.Parse(Puzzles.EmptyBoard).Candidates(4, 4));
    }

    [Fact]
    public void CandidatesCanBeEmptyForAnEmptyCell()
    {
        Assert.Empty(SudokuBoard.Parse(Puzzles.NoCandidate).Candidates(0, 8));
    }

    [Theory]
    [InlineData(Puzzles.Wikipedia)]
    [InlineData(Puzzles.WikipediaSolution)]
    [InlineData(Puzzles.ManyDeadEnds)]
    [InlineData(Puzzles.NoSolutionDeep)]
    [InlineData(Puzzles.NoCandidate)]
    public void IsValidAcceptsBoardsWithoutConflicts(string puzzle)
    {
        Assert.True(SudokuBoard.Parse(puzzle).IsValid());
    }

    [Fact]
    public void IsValidAcceptsTheEmptyBoard()
    {
        Assert.True(SudokuBoard.Parse(Puzzles.EmptyBoard).IsValid());
    }

    [Fact]
    public void IsValidRejectsTwoEqualDigitsInARow()
    {
        var board = SudokuBoard.Parse(Puzzles.EmptyBoard);
        board[3, 0] = 7;
        board[3, 8] = 7;
        Assert.False(board.IsValid());
    }

    [Fact]
    public void IsValidRejectsTwoEqualDigitsInAColumn()
    {
        var board = SudokuBoard.Parse(Puzzles.EmptyBoard);
        board[0, 5] = 2;
        board[8, 5] = 2;
        Assert.False(board.IsValid());
    }

    [Fact]
    public void IsValidRejectsTwoEqualDigitsInABox()
    {
        // Different row and different column, but the same box (rows 6–8, columns 6–8).
        var board = SudokuBoard.Parse(Puzzles.EmptyBoard);
        board[6, 6] = 9;
        board[8, 7] = 9;
        Assert.False(board.IsValid());
    }

    [Fact]
    public void IsValidRejectsAConflictWithAGivenDigit()
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        board[0, 2] = 8;   // column 2 already contains an 8
        Assert.False(board.IsValid());
    }

    [Fact]
    public void IsCompleteChecksOnlyForEmptyCells()
    {
        Assert.False(_wikipedia.IsComplete());
        Assert.True(SudokuBoard.Parse(Puzzles.WikipediaSolution).IsComplete());
        var allOnes = SudokuBoard.Parse(new string('1', 81));
        Assert.True(allOnes.IsComplete());
        Assert.False(allOnes.IsValid());
    }
}
