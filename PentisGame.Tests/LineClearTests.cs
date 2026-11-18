using PentisGame.Core;

namespace PentisGame.Tests;

public class LineClearTests
{
    private static int Bottom => GameConstants.BoardHeight + GameConstants.HiddenRows - 1;

    [Fact]
    public void SingleFullLine_DisappearsAndRowsAboveFall()
    {
        var board = new Board();
        FillRow(board, Bottom, 1);
        board.SetCell(0, Bottom - 1, 2);

        Assert.Equal(1, board.ClearFullLines());
        Assert.Equal(2, board.GetCell(0, Bottom));
        Assert.Equal(0, board.GetCell(1, Bottom));
        Assert.Equal(0, board.GetCell(0, Bottom - 1));
        Assert.Empty(board.FindFullRows());
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void MultipleFullLines_ClearTogether(int lineCount)
    {
        var board = new Board();
        for (var i = 0; i < lineCount; i++)
        {
            FillRow(board, Bottom - i, 1);
        }

        board.SetCell(5, Bottom - lineCount, 7);
        Assert.Equal(lineCount, board.ClearFullLines());
        Assert.Equal(7, board.GetCell(5, Bottom));
        Assert.Empty(board.FindFullRows());
    }

    [Fact]
    public void NonAdjacentFullLines_KeepIncompleteRowIntact()
    {
        var board = new Board();
        FillRow(board, Bottom, 1);
        FillRow(board, Bottom - 2, 2);
        board.SetCell(0, Bottom - 1, 3);

        Assert.Equal(2, board.ClearFullLines());
        Assert.Equal(3, board.GetCell(0, Bottom));
        Assert.Equal(0, board.GetCell(1, Bottom));
        Assert.Equal(0, board.GetCell(0, Bottom - 1));
    }

    [Fact]
    public void BottomAndMiddleLines_ClearCorrectly()
    {
        var board = new Board();
        FillRow(board, Bottom, 1);
        FillRow(board, Bottom - 6, 2);
        board.SetCell(4, Bottom - 7, 5);
        board.SetCell(4, Bottom - 1, 6);

        Assert.Equal(2, board.ClearFullLines());
        Assert.Equal(6, board.GetCell(4, Bottom));
        Assert.Equal(5, board.GetCell(4, Bottom - 5));
        Assert.Empty(board.FindFullRows());
    }

    [Fact]
    public void AfterClear_EmptyRowsOnlyAboveRemainingContent()
    {
        var board = new Board();
        FillRow(board, Bottom, 1);
        FillRow(board, Bottom - 1, 1);
        board.SetCell(2, Bottom - 3, 4);

        board.ClearFullLines();
        Assert.Equal(4, board.GetCell(2, Bottom - 1));
        for (var y = 0; y < board.TotalRows; y++)
        {
            for (var x = 0; x < 10; x++)
            {
                if (y == Bottom - 1 && x == 2)
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
