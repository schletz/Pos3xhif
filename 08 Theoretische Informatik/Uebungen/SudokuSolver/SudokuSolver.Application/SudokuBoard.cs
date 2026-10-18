namespace SudokuSolver.Application;

/// <summary>
/// A 9×9 sudoku board. Every cell contains a digit from 1 to 9 or <see cref="Empty"/>.
/// Rows and columns are numbered from 0 to 8, starting at the top left.
/// </summary>
public sealed class SudokuBoard
{
    /// <summary>Number of rows and columns.</summary>
    public const int Size = 9;

    /// <summary>Value of an empty cell.</summary>
    public const int Empty = 0;

    private readonly int[,] _cells = new int[Size, Size];

    private SudokuBoard()
    {
    }

    /// <summary>
    /// Reads a board from a string with 81 cells, row by row. Whitespace (spaces, line breaks) is ignored.
    /// The digits 1 to 9 are filled cells, <c>0</c> and <c>.</c> are empty cells.
    /// The sudoku rules are not checked (use <see cref="IsValid"/> for that).
    /// </summary>
    /// <param name="text">The board, e.g. <c>"53..7....6..195..."</c>.</param>
    /// <returns>The new board.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="text"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException">The text does not contain exactly 81 cells or contains an invalid character.</exception>
    public static SudokuBoard Parse(string text)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets or sets the value of a cell (1 to 9, or <see cref="Empty"/>).
    /// </summary>
    /// <param name="row">Row from 0 to 8.</param>
    /// <param name="column">Column from 0 to 8.</param>
    /// <exception cref="ArgumentOutOfRangeException">The row, the column or the new value is out of range.</exception>
    public int this[int row, int column]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <summary>
    /// Checks whether <paramref name="digit"/> may be placed in the given cell: it must not occur in the
    /// same row, the same column or the same 3×3 box. The cell itself is not taken into account, so the
    /// method also works for a cell that is already filled.
    /// </summary>
    /// <param name="row">Row from 0 to 8.</param>
    /// <param name="column">Column from 0 to 8.</param>
    /// <param name="digit">Digit from 1 to 9.</param>
    /// <returns><c>true</c> if the digit does not cause a conflict.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The row, the column or the digit is out of range.</exception>
    public bool CanPlace(int row, int column, int digit)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns all digits that may be placed in an empty cell (see <see cref="CanPlace"/>), in ascending order.
    /// For a filled cell the list is empty.
    /// </summary>
    /// <param name="row">Row from 0 to 8.</param>
    /// <param name="column">Column from 0 to 8.</param>
    /// <exception cref="ArgumentOutOfRangeException">The row or the column is out of range.</exception>
    public IReadOnlyList<int> Candidates(int row, int column)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks the sudoku rules: no digit occurs twice in a row, a column or a 3×3 box.
    /// Empty cells are ignored, so a board that is not yet complete can be valid.
    /// </summary>
    public bool IsValid()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Checks whether every cell is filled. The rules are not checked.
    /// </summary>
    public bool IsComplete()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Creates an independent copy of the board. Changing the copy does not change the original.
    /// </summary>
    public SudokuBoard Clone()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Returns the board as a string of 81 characters, row by row, without whitespace.
    /// Empty cells are written as <c>.</c>.
    /// </summary>
    public override string ToString()
    {
        throw new NotImplementedException();
    }
}
