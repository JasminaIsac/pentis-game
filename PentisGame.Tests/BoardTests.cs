using PentisGame.Core;

namespace PentisGame.Tests;

public class BoardTests
{
    [Fact]
    public void CellOutsideColumnsOrBelowFloor_IsInvalid()
    {
        var board = new Board();
        Assert.False(board.IsValidPosition(new Pentomino(PentominoType.X, -1, 10)));
        Assert.False(board.IsValidPosition(new Pentomino(PentominoType.X, 8, 10)));
        // X pe 3 rânduri: la Y = Bottom-1 iese sub bază
        var bottom = GameConstants.BoardHeight + GameConstants.HiddenRows - 1;
        Assert.False(board.IsValidPosition(new Pentomino(PentominoType.X, 4, bottom - 1)));
    }

    [Fact]
    public void PieceOnlyInHiddenRows_IsValid()
    {
        var board = new Board();
        // X pe 3×3 la Y=0 stă în rândurile 0–2 (ascunse când HiddenRows=5)
        var piece = new Pentomino(PentominoType.X, 3, 0);
        Assert.True(board.IsValidPosition(piece));
        Assert.All(piece.GetCells(), cell => Assert.True(cell.Y < GameConstants.HiddenRows));
    }

    [Fact]
    public void LockPiece_WritesColorIdAndLeavesOtherCells()
    {
        var board = new Board();
        var piece = new Pentomino(PentominoType.X, 4, 19);
        board.LockPiece(piece);

        var expected = PentominoCatalog.GetColorId(PentominoType.X);
        foreach (var (x, y) in piece.GetCells())
        {
            Assert.Equal(expected, board.GetCell(x, y));
        }

        Assert.Equal(0, board.GetCell(0, 10));
    }

    [Fact]
    public void OverlapWithLockedCell_IsInvalid()
    {
        var board = new Board();
        board.LockPiece(new Pentomino(PentominoType.X, 4, 18));
        Assert.False(board.IsValidPosition(new Pentomino(PentominoType.X, 4, 17)));
    }

    [Fact]
    public void Clear_LeavesAllCellsEmpty()
    {
        var board = new Board();
        board.LockPiece(new Pentomino(PentominoType.I, 2, 0));
        board.Clear();

        for (var y = 0; y < board.TotalRows; y++)
        {
            for (var x = 0; x < board.Width; x++)
            {
                Assert.Equal(0, board.GetCell(x, y));
            }
        }
    }

    [Fact]
    public void IsInside_CoversFullGridIncludingHiddenRows()
    {
        var board = new Board();
        Assert.True(board.IsInside(0, 0));
        Assert.True(board.IsInside(9, board.TotalRows - 1));
        Assert.False(board.IsInside(-1, 0));
        Assert.False(board.IsInside(10, 0));
        Assert.False(board.IsInside(0, board.TotalRows));
    }
}
