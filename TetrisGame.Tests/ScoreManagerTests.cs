using TetrisGame.Core;

namespace TetrisGame.Tests;

public class ScoreManagerTests
{
    [Theory]
    [InlineData(1, 100)]
    [InlineData(2, 300)]
    [InlineData(3, 500)]
    [InlineData(4, 800)]
    public void Level1_LineClearsUseClassicTable(int lines, int expectedScore)
    {
        var score = new ScoreManager();
        score.Reset(1);
        score.AddLineClear(lines);
        Assert.Equal(expectedScore, score.Score);
    }

    [Fact]
    public void Level3_FourLinesAward2400()
    {
        var score = new ScoreManager();
        score.Reset(3);
        score.AddLineClear(4);
        Assert.Equal(2400, score.Score);
    }

    [Fact]
    public void TenthLine_RaisesLevelAfterPointsUseOldLevel()
    {
        var score = new ScoreManager();
        score.Reset(1);
        score.AddLineClear(4);
        score.AddLineClear(4);
        score.AddLineClear(1);

        Assert.Equal(1, score.Level);
        Assert.Equal(1700, score.Score);

        var levels = new List<int>();
        score.LevelChanged += () => levels.Add(score.Level);
        score.AddLineClear(1);

        Assert.Equal(2, score.Level);
        Assert.Equal(1800, score.Score);
        Assert.Equal([2], levels);
    }

    [Fact]
    public void StartLevel5_ReachesLevel6AfterTenLines()
    {
        var score = new ScoreManager();
        score.Reset(5);
        Assert.Equal(5, score.Level);

        for (var i = 0; i < 5; i++)
        {
            score.AddLineClear(2);
        }

        Assert.Equal(6, score.Level);
        Assert.Equal(10, score.TotalLines);
    }

    [Fact]
    public void DropInterval_FollowsFormulaAndFloor()
    {
        var score = new ScoreManager();
        score.Reset(1);
        Assert.Equal(800, score.DropIntervalMs);

        score.Reset(2);
        Assert.Equal(720, score.DropIntervalMs);

        score.Reset(3);
        Assert.Equal(648, score.DropIntervalMs);

        score.Reset(1);
        for (var i = 0; i < 50; i++)
        {
            score.AddLineClear(4);
        }

        Assert.True(score.Level > 10);
        Assert.Equal(100, score.DropIntervalMs);
    }

    [Fact]
    public void SoftAndHardDrop_AddExpectedPoints()
    {
        var score = new ScoreManager();
        score.Reset(1);
        score.AddSoftDrop(4);
        Assert.Equal(4, score.Score);
        score.AddHardDrop(4);
        Assert.Equal(12, score.Score);
    }

    [Fact]
    public void Events_FireOnlyWhenValuesChange()
    {
        var score = new ScoreManager();
        score.Reset(1);
        var scoreEvents = 0;
        var levelEvents = 0;
        score.ScoreChanged += () => scoreEvents++;
        score.LevelChanged += () => levelEvents++;

        score.AddSoftDrop(0);
        Assert.Equal(0, scoreEvents);

        score.AddSoftDrop(1);
        Assert.Equal(1, scoreEvents);
        Assert.Equal(0, levelEvents);

        score.AddLineClear(1);
        Assert.Equal(2, scoreEvents);
        Assert.Equal(0, levelEvents);
    }
}
