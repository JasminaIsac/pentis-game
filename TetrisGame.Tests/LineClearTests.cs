using TetrisGame.Core;

namespace TetrisGame.Tests;

public class LineClearTests
{
    [Fact]
    public void SingleFullLine_DisappearsAndRowsAboveFall()
    {
        var board = new Board();
        FillRow(board, 21, 1);
        board.SetCell(0, 20, 2);

        Assert.Equal(1, board.ClearFullLines());

        Assert.Equal(2, board.GetCell(0, 21));
        Assert.Equal(0, board.GetCell(1, 21));
        Assert.Equal(0, board.GetCell(0, 20));
        Assert.Empty(board.FindFullRows());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void MultipleFullLines_ClearTogether(int lineCount)
    {
        var board = new Board();
        for (var i = 0; i < lineCount; i++)
        {
            FillRow(board, 21 - i, 1);
        }

        board.SetCell(5, 21 - lineCount, 7);

        Assert.Equal(lineCount, board.ClearFullLines());
        Assert.Equal(7, board.GetCell(5, 21));
        Assert.Empty(board.FindFullRows());
    }

    [Fact]
    public void NonAdjacentFullLines_KeepIncompleteRowIntact()
    {
        var board = new Board();
        FillRow(board, 21, 1);
        FillRow(board, 19, 2);
        board.SetCell(0, 20, 3);

        Assert.Equal(2, board.ClearFullLines());
        Assert.Equal(3, board.GetCell(0, 21));
        Assert.Equal(0, board.GetCell(1, 21));
        Assert.Equal(0, board.GetCell(0, 20));
    }

    [Fact]
    public void BottomAndMiddleLines_ClearCorrectly()
    {
        var board = new Board();
        FillRow(board, 21, 1);
        FillRow(board, 15, 2);
        board.SetCell(4, 14, 5);
        board.SetCell(4, 20, 6);

        Assert.Equal(2, board.ClearFullLines());
        Assert.Equal(6, board.GetCell(4, 21));
        Assert.Equal(5, board.GetCell(4, 16));
        Assert.Equal(0, board.GetCell(4, 15));
        Assert.Empty(board.FindFullRows());
    }

    [Fact]
    public void AfterClear_EmptyRowsOnlyAboveRemainingContent()
    {
        var board = new Board();
        FillRow(board, 21, 1);
        FillRow(board, 20, 1);
        board.SetCell(2, 18, 4);

        board.ClearFullLines();

        Assert.Equal(4, board.GetCell(2, 20));
        for (var y = 0; y < board.TotalRows; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                if (y == 20 && x == 2)
                {
                    continue;
                }

                Assert.Equal(0, board.GetCell(x, y));
            }
        }
    }

    private static void FillRow(Board board, int row, int value)
    {
        for (var x = 0; x < board.Width; x++)
        {
            board.SetCell(x, row, value);
        }
    }
}
