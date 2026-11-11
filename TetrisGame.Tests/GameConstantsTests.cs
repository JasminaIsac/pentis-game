using TetrisGame.Core;

namespace TetrisGame.Tests;

public class GameConstantsTests
{
    [Fact]
    public void Board_HasTenColumnsTwentyRowsAndTwoHiddenRows()
    {
        Assert.Equal(10, GameConstants.BoardWidth);
        Assert.Equal(20, GameConstants.BoardHeight);
        Assert.Equal(2, GameConstants.HiddenRows);
        Assert.Equal(30, GameConstants.CellSize);
    }

    [Fact]
    public void DropInterval_StartsAt800AndStopsAt100()
    {
        Assert.Equal(800, GameConstants.InitialDropIntervalMs);
        Assert.Equal(100, GameConstants.MinDropIntervalMs);
        Assert.Equal(10, GameConstants.LinesPerLevel);
    }

    [Fact]
    public void LineClearScores_UseTheClassicTable()
    {
        Assert.Equal([0, 100, 300, 500, 800], GameConstants.LineClearScores);
    }
}
