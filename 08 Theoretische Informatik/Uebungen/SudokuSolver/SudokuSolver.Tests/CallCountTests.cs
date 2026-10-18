using SudokuSolver.Application;

namespace SudokuSolver.Tests;

/// <summary>
/// Tests the number of recursive calls in <see cref="SolveResult.Calls"/>. The exact number depends on details
/// of the implementation, so the tests only check bounds and the comparison of the two selection rules.
/// </summary>
public class CallCountTests
{
    [Theory]
    [InlineData(Puzzles.Wikipedia, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.Wikipedia, CellSelection.FewestCandidates)]
    [InlineData(Puzzles.ManyDeadEnds, CellSelection.FirstEmpty)]
    [InlineData(Puzzles.ManyDeadEnds, CellSelection.FewestCandidates)]
    public void EveryEmptyCellNeedsAtLeastOneCall(string puzzle, CellSelection selection)
    {
        // One call per placed digit plus the first call, which finds no empty cell at the end.
        var minimum = Puzzles.CountEmpty(puzzle) + 1;
        var result = Solver.Solve(SudokuBoard.Parse(puzzle), selection);
        Assert.True(result.Calls >= minimum, $"Expected at least {minimum} calls, but got {result.Calls}.");
    }

    [Fact]
    public void FewestCandidatesSolvesTheWikipediaPuzzleWithoutGoingBack()
    {
        // In this puzzle there is always a cell with exactly one candidate.
        var result = Solver.Solve(SudokuBoard.Parse(Puzzles.Wikipedia), CellSelection.FewestCandidates);
        Assert.Equal(Puzzles.CountEmpty(Puzzles.Wikipedia) + 1, result.Calls);
    }

    [Fact]
    public void FirstEmptyHasToGoBackInTheWikipediaPuzzle()
    {
        var result = Solver.Solve(SudokuBoard.Parse(Puzzles.Wikipedia), CellSelection.FirstEmpty);
        Assert.True(result.Calls > Puzzles.CountEmpty(Puzzles.Wikipedia) + 1,
            $"Filling row by row runs into dead ends, but the solver needed only {result.Calls} calls.");
    }

    [Theory]
    [InlineData(Puzzles.Wikipedia)]
    [InlineData(Puzzles.ManyDeadEnds)]
    [InlineData(Puzzles.NoSolutionDeep)]
    public void FewestCandidatesNeedsFewerCalls(string puzzle)
    {
        var board = SudokuBoard.Parse(puzzle);
        var firstEmpty = Solver.Solve(board, CellSelection.FirstEmpty).Calls;
        var fewestCandidates = Solver.Solve(board, CellSelection.FewestCandidates).Calls;
        Assert.True(fewestCandidates < firstEmpty,
            $"FewestCandidates needed {fewestCandidates} calls, FirstEmpty {firstEmpty}.");
    }

    [Fact]
    public void FewestCandidatesNeedsAtLeastTenTimesFewerCallsForManyDeadEnds()
    {
        var board = SudokuBoard.Parse(Puzzles.ManyDeadEnds);
        var firstEmpty = Solver.Solve(board, CellSelection.FirstEmpty).Calls;
        var fewestCandidates = Solver.Solve(board, CellSelection.FewestCandidates).Calls;
        Assert.True(fewestCandidates * 10 <= firstEmpty,
            $"FewestCandidates needed {fewestCandidates} calls, FirstEmpty {firstEmpty}.");
    }
}
