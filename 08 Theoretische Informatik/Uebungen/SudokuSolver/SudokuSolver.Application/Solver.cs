namespace SudokuSolver.Application;

/// <summary>
/// Solves sudoku puzzles with backtracking and counts the recursive calls.
/// </summary>
public static class Solver
{
    /// <summary>
    /// Solves <paramref name="board"/> with backtracking. The board passed in is not changed.
    /// If the board breaks the rules from the start (see <see cref="SudokuBoard.IsValid"/>),
    /// the result has no solution and 0 calls.
    /// </summary>
    /// <param name="board">The puzzle.</param>
    /// <param name="selection">Rule for choosing the next empty cell.</param>
    /// <returns>The solution (or <c>null</c>) and the number of recursive calls.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="board"/> is <c>null</c>.</exception>
    public static SolveResult Solve(SudokuBoard board, CellSelection selection = CellSelection.FirstEmpty)
    {
        throw new NotImplementedException();
    }
}
