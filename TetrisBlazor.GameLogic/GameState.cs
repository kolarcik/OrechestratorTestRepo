namespace TetrisBlazor.Client.GameLogic;

public enum GameStatus { Idle, Playing, Paused, GameOver }

public class GameState
{
    public GameStatus Status { get; set; } = GameStatus.Idle;
    public Board Board { get; } = new();
    public ScoreManager ScoreManager { get; } = new();
    public Tetromino? CurrentPiece { get; set; }
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }
    public Tetromino? NextPiece { get; set; }

    public void Reset()
    {
        Board.Reset();
        ScoreManager.Reset();
        CurrentPiece = null;
        NextPiece = null;
    }
}
