namespace TetrisBlazor.Client.GameLogic;

public class ScoreManager
{
    private static readonly int[] LinePoints = [0, 100, 300, 500, 800];

    public int Score { get; private set; }
    public int Level { get; private set; } = 1;
    public int Lines { get; private set; }

    public void Reset() { Score = 0; Level = 1; Lines = 0; }

    public void AddLines(int count)
    {
        Score += (count < LinePoints.Length ? LinePoints[count] : 800) * Level;
        Lines += count;
        Level = Lines / 10 + 1;
    }

    public void AddSoftDrop(int cells) => Score += cells;
    public void AddHardDrop(int cells) => Score += cells * 2;

    public int GetSpeedMs() => Math.Max(100, 1000 - (Level - 1) * 100);
}
