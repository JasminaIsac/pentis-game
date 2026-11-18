using TetrisGame.Core;

namespace TetrisGame.Tests;

public class BoardTests
{
    [Fact]
    public void CellOutsideColumnsOrBelowFloor_IsInvalid()
    {
        var board = new Board();
        var piece = new Tetromino(TetrominoType.O, -1, 10);
        Assert.False(board.IsValidPosition(piece));

        piece = new Tetromino(TetrominoType.O, 9, 10);
        Assert.False(board.IsValidPosition(piece));

        piece = new Tetromino(TetrominoType.O, 4, 21);
        Assert.False(board.IsValidPosition(piece));
    }

    [Fact]
    public void PieceOnlyInHiddenRows_IsValid()
    {
        var board = new Board();
        var piece = new Tetromino(TetrominoType.O, 4, 0);
        Assert.True(board.IsValidPosition(piece));
        Assert.All(piece.GetCells(), cell => Assert.True(cell.Y is 0 or 1));
    }

    [Fact]
    public void LockPiece_WritesColorIdAndLeavesOtherCells()
    {
        var board = new Board();
        board.LockPiece(new Tetromino(TetrominoType.O, 0, 20));

        var expected = TetrominoCatalog.GetColorId(TetrominoType.O);
        Assert.Equal(expected, board.GetCell(0, 20));
        Assert.Equal(expected, board.GetCell(1, 20));
        Assert.Equal(expected, board.GetCell(0, 21));
        Assert.Equal(expected, board.GetCell(1, 21));
        Assert.Equal(0, board.GetCell(2, 20));
        Assert.Equal(0, board.GetCell(5, 10));
    }

    [Fact]
    public void OverlapWithLockedCell_IsInvalid()
    {
        var board = new Board();
        board.LockPiece(new Tetromino(TetrominoType.O, 4, 18));
        var overlapping = new Tetromino(TetrominoType.O, 4, 17);
        Assert.False(board.IsValidPosition(overlapping));
    }

    [Fact]
    public void Clear_LeavesAllCellsEmpty()
    {
        var board = new Board();
        board.LockPiece(new Tetromino(TetrominoType.I, 3, 0));
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
        Assert.True(board.IsInside(9, 21));
        Assert.False(board.IsInside(-1, 0));
        Assert.False(board.IsInside(10, 0));
        Assert.False(board.IsInside(0, 22));
    }
}
