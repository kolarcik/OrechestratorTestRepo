namespace TetrisBlazor.Client.GameLogic;

public enum TetrominoType { I, O, T, S, Z, J, L }

public class Tetromino
{
    public TetrominoType Type { get; }
    public string Color { get; }
    public int[,] Shape { get; private set; }

    private static readonly Dictionary<TetrominoType, (int[,] shape, string color)> Definitions = new()
    {
        [TetrominoType.I] = (new[,] { {0,0,0,0},{1,1,1,1},{0,0,0,0},{0,0,0,0} }, "#00f0f0"),
        [TetrominoType.O] = (new[,] { {1,1},{1,1} }, "#f0f000"),
        [TetrominoType.T] = (new[,] { {0,1,0},{1,1,1},{0,0,0} }, "#a000f0"),
        [TetrominoType.S] = (new[,] { {0,1,1},{1,1,0},{0,0,0} }, "#00f000"),
        [TetrominoType.Z] = (new[,] { {1,1,0},{0,1,1},{0,0,0} }, "#f00000"),
        [TetrominoType.J] = (new[,] { {1,0,0},{1,1,1},{0,0,0} }, "#0000f0"),
        [TetrominoType.L] = (new[,] { {0,0,1},{1,1,1},{0,0,0} }, "#f0a000"),
    };

    public Tetromino(TetrominoType type)
    {
        Type = type;
        var def = Definitions[type];
        Color = def.color;
        int rows = def.shape.GetLength(0), cols = def.shape.GetLength(1);
        Shape = new int[rows, cols];
        Array.Copy(def.shape, Shape, def.shape.Length);
    }

    public static Tetromino Random()
    {
        var types = Enum.GetValues<TetrominoType>();
        return new Tetromino(types[System.Random.Shared.Next(types.Length)]);
    }

    public void RotateCW()
    {
        int n = Shape.GetLength(0), m = Shape.GetLength(1);
        var result = new int[m, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < m; c++)
                result[c, n - 1 - r] = Shape[r, c];
        Shape = result;
    }

    public void RotateCCW()
    {
        int n = Shape.GetLength(0), m = Shape.GetLength(1);
        var result = new int[m, n];
        for (int r = 0; r < n; r++)
            for (int c = 0; c < m; c++)
                result[m - 1 - c, r] = Shape[r, c];
        Shape = result;
    }

    public int[,] GetRotatedCW()
    {
        var copy = Clone();
        copy.RotateCW();
        return copy.Shape;
    }

    public int[,] GetRotatedCCW()
    {
        var copy = Clone();
        copy.RotateCCW();
        return copy.Shape;
    }

    public Tetromino Clone()
    {
        var t = new Tetromino(Type);
        t.Shape = (int[,])Shape.Clone();
        return t;
    }
}
