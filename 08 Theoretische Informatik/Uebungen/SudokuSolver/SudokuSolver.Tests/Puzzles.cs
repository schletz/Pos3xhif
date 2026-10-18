namespace SudokuSolver.Tests;

/// <summary>
/// Puzzles used by the tests. Every puzzle whose solution is compared cell by cell has exactly one solution.
/// </summary>
internal static class Puzzles
{
    /// <summary>The example puzzle from the German and English Wikipedia article (public domain). 51 empty cells.</summary>
    public const string Wikipedia =
        "53..7....6..195....98....6.8...6...34..8.3..17...2...6.6....28....419..5....8..79";

    /// <summary>The only solution of <see cref="Wikipedia"/>.</summary>
    public const string WikipediaSolution =
        "534678912672195348198342567859761423426853791713924856961537284287419635345286179";

    /// <summary>
    /// A generated puzzle with 56 empty cells. Filling the first empty cell leads to a lot of backtracking,
    /// choosing the cell with the fewest candidates hardly ever has to go back.
    /// </summary>
    public const string ManyDeadEnds =
        "..6958..7...1...2...4......35......2...........82..34.....1..6..25.67.987...9....";

    /// <summary>The only solution of <see cref="ManyDeadEnds"/>.</summary>
    public const string ManyDeadEndsSolution =
        "216958437879143526534726981357489612142635879698271345983512764425367198761894253";

    /// <summary>
    /// <see cref="Wikipedia"/> with an additional 3 in row 8, column 2. The board is valid, but the 3 does not
    /// belong there, so there is no solution. The solver only notices that after many placements.
    /// </summary>
    public const string NoSolutionDeep =
        "53..7....6..195....98....6.8...6...34..8.3..17...2...6.6....28....419..5..3.8..79";

    /// <summary>
    /// Valid, but the cell in row 0, column 8 has no candidate: the row already contains 1 to 8 and
    /// the column contains the 9.
    /// </summary>
    public const string NoCandidate =
        "12345678." +
        "........." +
        "........." +
        "........." +
        "........." +
        "........9" +
        "........." +
        "........." +
        ".........";

    /// <summary>81 empty cells.</summary>
    public static readonly string EmptyBoard = new('.', 81);

    /// <summary>Counts the empty cells of a puzzle string.</summary>
    public static int CountEmpty(string puzzle) => puzzle.Count(c => c is '.' or '0');
}
