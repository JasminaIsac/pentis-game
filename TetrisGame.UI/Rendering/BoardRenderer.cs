using TetrisGame.Core;

namespace TetrisGame.UI.Rendering;

/// <summary>
/// Desenează puțul, piesele fixate și piesa curentă. Pensulele sunt reutilizate.
/// </summary>
public sealed class BoardRenderer : IDisposable
{
    private readonly Dictionary<int, SolidBrush> _pieceBrushes = new();
    private readonly Dictionary<int, SolidBrush> _dimmedBrushes = new();
    private readonly SolidBrush _boardBrush;
    private readonly Pen _gridPen;
    private readonly Pen _cellBorderPen;
    private bool _disposed;

    /// <summary>
    /// Creează renderer-ul pentru tema dată.
    /// </summary>
    public BoardRenderer(Theme theme)
    {
        _boardBrush = new SolidBrush(theme.BoardBackground);
        _gridPen = new Pen(theme.Grid);
        _cellBorderPen = new Pen(theme.CellBorder);

        for (var id = 1; id <= 7; id++)
        {
            var color = theme.GetPieceColor(id);
            _pieceBrushes[id] = new SolidBrush(color);
            _dimmedBrushes[id] = new SolidBrush(Color.FromArgb(110, color.R, color.G, color.B));
        }
    }

    /// <summary>
    /// Desenează puțul în <paramref name="area"/> (doar rândurile vizibile).
    /// </summary>
    public void Draw(Graphics g, GameEngine engine, Rectangle area)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        g.FillRectangle(_boardBrush, area);

        var cell = GameConstants.CellSize;
        for (var row = 0; row <= GameConstants.BoardHeight; row++)
        {
            var y = area.Top + row * cell;
            g.DrawLine(_gridPen, area.Left, y, area.Right, y);
        }

        for (var col = 0; col <= GameConstants.BoardWidth; col++)
        {
            var x = area.Left + col * cell;
            g.DrawLine(_gridPen, x, area.Top, x, area.Bottom);
        }

        for (var boardRow = GameConstants.HiddenRows; boardRow < engine.Board.TotalRows; boardRow++)
        {
            var visibleRow = boardRow - GameConstants.HiddenRows;
            for (var col = 0; col < GameConstants.BoardWidth; col++)
            {
                var value = engine.Board.GetCell(col, boardRow);
                if (value == TetrominoCatalog.EmptyCell)
                {
                    continue;
                }

                DrawCell(g, area, col, visibleRow, value);
            }
        }

        if (engine.CurrentPiece is { } piece && engine.State != GameState.Menu)
        {
            var colorId = TetrominoCatalog.GetColorId(piece.Type);
            foreach (var (x, y) in piece.GetCells())
            {
                if (y < GameConstants.HiddenRows)
                {
                    continue;
                }

                DrawCell(g, area, x, y - GameConstants.HiddenRows, colorId);
            }
        }
    }

    /// <summary>
    /// Desenează o piesă miniatură centrată în casetă (NEXT / HOLD).
    /// </summary>
    public void DrawPreview(Graphics g, Rectangle box, TetrominoType? type, bool dimmed = false)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (type is null)
        {
            return;
        }

        var shape = TetrominoCatalog.GetShape(type.Value);
        var size = shape.GetLength(0);
        var mini = Math.Max(12, Math.Min(box.Width, box.Height) / (size + 1));

        var occupied = new List<(int Col, int Row)>();
        for (var row = 0; row < size; row++)
        {
            for (var col = 0; col < size; col++)
            {
                if (shape[row, col] != 0)
                {
                    occupied.Add((col, row));
                }
            }
        }

        var minCol = occupied.Min(c => c.Col);
        var maxCol = occupied.Max(c => c.Col);
        var minRow = occupied.Min(c => c.Row);
        var maxRow = occupied.Max(c => c.Row);
        var pieceWidth = (maxCol - minCol + 1) * mini;
        var pieceHeight = (maxRow - minRow + 1) * mini;
        var originX = box.Left + (box.Width - pieceWidth) / 2;
        var originY = box.Top + (box.Height - pieceHeight) / 2;

        var colorId = TetrominoCatalog.GetColorId(type.Value);
        var brushes = dimmed ? _dimmedBrushes : _pieceBrushes;
        if (!brushes.TryGetValue(colorId, out var brush))
        {
            return;
        }

        foreach (var (col, row) in occupied)
        {
            var rect = new Rectangle(
                originX + (col - minCol) * mini,
                originY + (row - minRow) * mini,
                mini,
                mini);
            g.FillRectangle(brush, rect);
            g.DrawRectangle(_cellBorderPen, rect);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _boardBrush.Dispose();
        _gridPen.Dispose();
        _cellBorderPen.Dispose();
        foreach (var brush in _pieceBrushes.Values)
        {
            brush.Dispose();
        }

        foreach (var brush in _dimmedBrushes.Values)
        {
            brush.Dispose();
        }

        _disposed = true;
    }

    private void DrawCell(Graphics g, Rectangle area, int col, int visibleRow, int colorId)
    {
        var cell = GameConstants.CellSize;
        var rect = new Rectangle(
            area.Left + col * cell,
            area.Top + visibleRow * cell,
            cell,
            cell);

        if (!_pieceBrushes.TryGetValue(colorId, out var brush))
        {
            return;
        }

        g.FillRectangle(brush, rect);
        g.DrawRectangle(_cellBorderPen, rect);
    }
}
