using SudokuSolver.Application;

namespace SudokuSolver.Tests;

/// <summary>
/// Tests the results of <see cref="Solver.Solve"/> for both cell selection rules.
/// </summary>
public class SolverTests
{
    [Theory]
    [InlineData(Puzzles.Wikipedia, Puzzles.WikipediaSolution, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.Wikipedia, Puzzles.WikipediaSolution, CellSelection.FewestCandidates)]
    [InlineData(Puzzles.ManyDeadEnds, Puzzles.ManyDeadEndsSolution, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.ManyDeadEnds, Puzzles.ManyDeadEndsSolution, CellSelection.FewestCandidates)]
    public void SolveFindsTheOnlySolution(string puzzle, string solution, CellSelection selection)
    {
        var result = Solver.Solve(SudokuBoard.Parse(puzzle), selection);
        Assert.True(result.IsSolved, "The puzzle has a solution.");
        Assert.Equal(solution, result.Solution!.ToString());
    }

    [Fact]
    public void SolveUsesFirstEmptyByDefault()
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        Assert.Equal(Solver.Solve(board, CellSelection.FirstEmpty).Calls, Solver.Solve(board).Calls);
    }

    [Theory]
    [InlineData(CellSelection.FirstEmpty)]
    [InlineData(CellSelection.FewestCandidates)]
    public void SolveFillsTheEmptyBoard(CellSelection selection)
    {
        // The empty board has many solutions, so only the rules are checked.
        var result = Solver.Solve(SudokuBoard.Parse(Puzzles.EmptyBoard), selection);
        Assert.True(result.IsSolved, "The empty board has a solution.");
        Assert.True(result.Solution!.IsComplete(), "The solution must not contain empty cells.");
        Assert.True(result.Solution.IsValid(), $"The solution breaks the rules: {result.Solution}");
    }

    [Theory]
    [InlineData(CellSelection.FirstEmpty)]
    [InlineData(CellSelection.FewestCandidates)]
    public void SolveDoesNotChangeTheBoardPassedIn(CellSelection selection)
    {
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        Solver.Solve(board, selection);
        Assert.Equal(Puzzles.Wikipedia, board.ToString());
    }

    [Theory]
    [InlineData(CellSelection.FirstEmpty)]
    [InlineData(CellSelection.FewestCandidates)]
    public void SolveReturnsASolvedBoardWithOneCall(CellSelection selection)
    {
        var board = SudokuBoard.Parse(Puzzles.WikipediaSolution);
        var result = Solver.Solve(board, selection);
        Assert.Equal(Puzzles.WikipediaSolution, result.Solution?.ToString());
        Assert.Equal(1, result.Calls);
    }

    [Theory]
    [InlineData(Puzzles.NoCandidate, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.NoCandidate, CellSelection.FewestCandidates)]
    [InlineData(Puzzles.NoSolutionDeep, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.NoSolutionDeep, CellSelection.FewestCandidates)]
    public void SolveReturnsNoSolutionForAValidButUnsolvablePuzzle(string puzzle, CellSelection selection)
    {
        var result = Solver.Solve(SudokuBoard.Parse(puzzle), selection);
        Assert.False(result.IsSolved, $"The puzzle has no solution, but the solver returned {result.Solution}.");
        Assert.Null(result.Solution);
        Assert.True(result.Calls >= 1, "The board is valid, so the search must start.");
    }

    [Fact]
    public void SolveNeedsManyCallsToFindOutThatThereIsNoSolution()
    {
        // The wrong 3 is in the last row, so filling row by row runs into it late.
        var result = Solver.Solve(SudokuBoard.Parse(Puzzles.NoSolutionDeep), CellSelection.FirstEmpty);
        Assert.False(result.IsSolved);
        Assert.True(result.Calls > 100, $"Expected more than 100 calls, but got {result.Calls}.");
    }

    [Theory]
    [InlineData(CellSelection.FirstEmpty)]
    [InlineData(CellSelection.FewestCandidates)]
    public void SolveRejectsAnInvalidBoardWithoutSearching(CellSelection selection)
    {
        // Two 5s in row 0. Without the check, the solver would fill the other cells and return a wrong "solution".
        var board = SudokuBoard.Parse(Puzzles.Wikipedia);
        board[0, 2] = 5;
        var result = Solver.Solve(board, selection);
        Assert.Null(result.Solution);
        Assert.Equal(0, result.Calls);
    }

    [Fact]
    public void SolveNullThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Solver.Solve(null!));
    }
}
