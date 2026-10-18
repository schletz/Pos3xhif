namespace SudokuSolver.Application;

/// <summary>
/// Result of <see cref="Solver.Solve"/>.
/// </summary>
/// <param name="Solution">The solved board, or <c>null</c> if the puzzle has no solution.</param>
/// <param name="Calls">Number of calls of the recursive backtracking method, including the first call.</param>
public sealed record SolveResult(SudokuBoard? Solution, long Calls)
{
    /// <summary><c>true</c> if a solution was found.</summary>
    public bool IsSolved => Solution is not null;
}
