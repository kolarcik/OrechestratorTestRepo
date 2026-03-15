namespace TetrisBlazor.Client.GameLogic;

public class Board
{
    public const int Rows = 20;
    public const int Cols = 10;

    private string?[,] _grid = new string?[Rows, Cols];

    public string? GetCell(int row, int col) => _grid[row, col];

    public void Reset() => _grid = new string?[Rows, Cols];

    public bool IsValid(int[,] shape, int posX, int posY)
    {
        for (int r = 0; r < shape.GetLength(0); r++)
        for (int c = 0; c < shape.GetLength(1); c++)
        {
            if (shape[r, c] == 0) continue;
            int nr = posY + r, nc = posX + c;
            if (nc < 0 || nc >= Cols) return false;
            if (nr >= Rows) return false;
            if (nr >= 0 && _grid[nr, nc] != null) return false;
        }
        return true;
    }

    public void Lock(int[,] shape, int posX, int posY, string color)
    {
        for (int r = 0; r < shape.GetLength(0); r++)
        for (int c = 0; c < shape.GetLength(1); c++)
        {
            if (shape[r, c] == 0) continue;
            int nr = posY + r, nc = posX + c;
            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols)
                _grid[nr, nc] = color;
        }
    }

    public int ClearLines()
    {
        int cleared = 0;
        for (int r = Rows - 1; r >= 0; r--)
        {
            bool full = true;
            for (int c = 0; c < Cols; c++)
                if (_grid[r, c] == null) { full = false; break; }

            if (!full) continue;

            for (int rr = r; rr > 0; rr--)
                for (int c = 0; c < Cols; c++)
                    _grid[rr, c] = _grid[rr - 1, c];
            for (int c = 0; c < Cols; c++)
                _grid[0, c] = null;

            cleared++;
            r++; // re-check same row index after shift
        }
        return cleared;
    }

    public int GetGhostY(int[,] shape, int posX, int posY)
    {
        int ghostY = posY;
        while (IsValid(shape, posX, ghostY + 1)) ghostY++;
        return ghostY;
    }
}
