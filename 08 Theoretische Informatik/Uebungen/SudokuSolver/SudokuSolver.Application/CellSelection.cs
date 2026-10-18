namespace SudokuSolver.Application;

/// <summary>
/// Rule that decides which empty cell the solver fills next.
/// </summary>
public enum CellSelection
{
    /// <summary>The first empty cell, read row by row from the top left (row 0, column 0) to the bottom right.</summary>
    FirstEmpty,

    /// <summary>The empty cell with the fewest candidates. On a tie the first of these cells, read row by row.</summary>
    FewestCandidates
}
