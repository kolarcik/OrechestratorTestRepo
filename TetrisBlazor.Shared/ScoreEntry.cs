namespace TetrisBlazor.Shared;

public class ScoreEntry
{
    public string Name { get; set; } = string.Empty;
    public int Score { get; set; }
    public int Level { get; set; }
    public int Lines { get; set; }
    public string Date { get; set; } = string.Empty;
}
