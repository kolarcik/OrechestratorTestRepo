using TetrisBlazor.Client.GameLogic;

namespace TetrisBlazor.Tests.Client;

public class ScoreManagerTests
{
    // --- AddLines ---

    [Fact]
    public void AddLines_1Line_Adds100PointsAtLevel1()
    {
        var sm = new ScoreManager();
        sm.AddLines(1);
        Assert.Equal(100, sm.Score);
    }

    [Fact]
    public void AddLines_2Lines_Adds300Points()
    {
        var sm = new ScoreManager();
        sm.AddLines(2);
        Assert.Equal(300, sm.Score);
    }

    [Fact]
    public void AddLines_3Lines_Adds500Points()
    {
        var sm = new ScoreManager();
        sm.AddLines(3);
        Assert.Equal(500, sm.Score);
    }

    [Fact]
    public void AddLines_4Lines_Adds800Points()
    {
        var sm = new ScoreManager();
        sm.AddLines(4);
        Assert.Equal(800, sm.Score);
    }

    [Fact]
    public void AddLines_ScoreMultipliedByLevel()
    {
        var sm = new ScoreManager();
        // Reach level 2 (10 lines)
        for (int i = 0; i < 10; i++) sm.AddLines(1);
        Assert.Equal(2, sm.Level);
        var scoreBefore = sm.Score;
        sm.AddLines(1);
        Assert.Equal(scoreBefore + 100 * 2, sm.Score); // 200 at level 2
    }

    // --- Level progression ---

    [Theory]
    [InlineData(0, 1)]
    [InlineData(9, 1)]
    [InlineData(10, 2)]
    [InlineData(19, 2)]
    [InlineData(20, 3)]
    public void Level_IncreasesEvery10Lines(int totalLines, int expectedLevel)
    {
        var sm = new ScoreManager();
        int remaining = totalLines;
        while (remaining >= 4) { sm.AddLines(4); remaining -= 4; }
        while (remaining > 0) { sm.AddLines(1); remaining--; }
        Assert.Equal(expectedLevel, sm.Level);
    }

    [Fact]
    public void AddLines_TracksLinesCount()
    {
        var sm = new ScoreManager();
        sm.AddLines(2);
        sm.AddLines(3);
        Assert.Equal(5, sm.Lines);
    }

    // --- Soft / Hard drop ---

    [Fact]
    public void AddSoftDrop_AddsOnePointPerCell()
    {
        var sm = new ScoreManager();
        sm.AddSoftDrop(5);
        Assert.Equal(5, sm.Score);
    }

    [Fact]
    public void AddHardDrop_AddsTwoPointsPerCell()
    {
        var sm = new ScoreManager();
        sm.AddHardDrop(10);
        Assert.Equal(20, sm.Score);
    }

    // --- GetSpeedMs ---

    [Theory]
    [InlineData(1, 1000)]
    [InlineData(2, 900)]
    [InlineData(5, 600)]
    [InlineData(10, 100)]
    [InlineData(11, 100)]  // minimum clamp
    [InlineData(20, 100)]  // minimum clamp
    public void GetSpeedMs_DecreasesWithLevel(int level, int expectedMs)
    {
        var sm = new ScoreManager();
        int linesToReach = (level - 1) * 10;
        int remaining = linesToReach;
        while (remaining >= 4) { sm.AddLines(4); remaining -= 4; }
        while (remaining > 0) { sm.AddLines(1); remaining--; }
        Assert.Equal(level, sm.Level);
        Assert.Equal(expectedMs, sm.GetSpeedMs());
    }

    // --- Reset ---

    [Fact]
    public void Reset_ClearsScoreLevelLines()
    {
        var sm = new ScoreManager();
        sm.AddLines(4);
        sm.AddSoftDrop(10);
        sm.Reset();
        Assert.Equal(0, sm.Score);
        Assert.Equal(1, sm.Level);
        Assert.Equal(0, sm.Lines);
    }
}
