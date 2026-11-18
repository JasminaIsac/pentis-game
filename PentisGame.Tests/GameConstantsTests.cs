using PentisGame.Core;

namespace PentisGame.Tests;

public class GameConstantsTests
{
    [Fact]
    public void Board_HasTenColumnsTwentyRowsAndFiveHiddenRows()
    {
        Assert.Equal(10, GameConstants.BoardWidth);
        Assert.Equal(20, GameConstants.BoardHeight);
        Assert.Equal(5, GameConstants.HiddenRows);
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
    public void LineClearScores_IncludePentisBonus()
    {
        Assert.Equal([0, 100, 300, 500, 800, 1200], GameConstants.LineClearScores);
    }
}
