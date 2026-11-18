namespace TetrisGame.Core;

/// <summary>
/// Puțul de joc: 10 coloane, 20 rânduri vizibile și 2 rânduri ascunse.
/// </summary>
public sealed class Board
{
    private readonly int[,] _cells =
        new int[GameConstants.BoardHeight + GameConstants.HiddenRows, GameConstants.BoardWidth];

    /// <summary>Numărul total de rânduri, inclusiv cele ascunse.</summary>
    public int TotalRows => GameConstants.BoardHeight + GameConstants.HiddenRows;

    /// <summary>Numărul de coloane.</summary>
    public int Width => GameConstants.BoardWidth;

    /// <summary>Adevărat dacă (x, y) este în interiorul grilei.</summary>
    public bool IsInside(int x, int y) =>
        x >= 0 && x < GameConstants.BoardWidth && y >= 0 && y < TotalRows;

    /// <summary>Celula este în interior și liberă (valoarea 0).</summary>
    public bool IsCellFree(int x, int y) =>
        IsInside(x, y) && _cells[y, x] == TetrominoCatalog.EmptyCell;

    /// <summary>
    /// Poziția este validă când toate cele 4 celule ocupate stau în puț și pe locuri libere.
    /// </summary>
    public bool IsValidPosition(Tetromino piece)
    {
        foreach (var (x, y) in piece.GetCells())
        {
            if (!IsCellFree(x, y))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Scrie identificatorul piesei în cele 4 celule. Se apelează doar pentru o poziție validă.
    /// </summary>
    public void LockPiece(Tetromino piece)
    {
        var colorId = TetrominoCatalog.GetColorId(piece.Type);
        foreach (var (x, y) in piece.GetCells())
        {
            _cells[y, x] = colorId;
        }
    }

    /// <summary>Valoarea din grilă, pentru desenare și teste.</summary>
    public int GetCell(int x, int y) => _cells[y, x];

    /// <summary>Scrie o valoare în grilă. Folosit de teste pentru aranjamente controlate.</summary>
    internal void SetCell(int x, int y, int value) => _cells[y, x] = value;

    /// <summary>Toate celulele redevin libere.</summary>
    public void Clear()
    {
        Array.Clear(_cells);
    }

    /// <summary>Indicii rândurilor în care toate cele 10 celule sunt ocupate.</summary>
    public IReadOnlyList<int> FindFullRows()
    {
        var rows = new List<int>();
        for (var row = 0; row < TotalRows; row++)
        {
            var full = true;
            for (var col = 0; col < Width; col++)
            {
                if (_cells[row, col] == TetrominoCatalog.EmptyCell)
                {
                    full = false;
                    break;
                }
            }

            if (full)
            {
                rows.Add(row);
            }
        }

        return rows;
    }

    /// <summary>
    /// Șterge rândurile primite și coboară conținutul de deasupra, păstrând ordinea.
    /// </summary>
    public void ClearRows(IReadOnlyList<int> rows)
    {
        if (rows.Count == 0)
        {
            return;
        }

        var toClear = new HashSet<int>(rows);
        var next = new int[TotalRows, Width];
        var writeRow = TotalRows - 1;

        for (var readRow = TotalRows - 1; readRow >= 0; readRow--)
        {
            if (toClear.Contains(readRow))
            {
                continue;
            }

            for (var col = 0; col < Width; col++)
            {
                next[writeRow, col] = _cells[readRow, col];
            }

            writeRow--;
        }

        Array.Copy(next, _cells, next.Length);
    }

    /// <summary>
    /// Găsește și elimină liniile complete. Întoarce numărul de linii (0–4).
    /// </summary>
    public int ClearFullLines()
    {
        var fullRows = FindFullRows();
        ClearRows(fullRows);
        return fullRows.Count;
    }
}
