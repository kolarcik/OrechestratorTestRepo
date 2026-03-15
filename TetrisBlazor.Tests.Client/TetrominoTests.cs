using TetrisBlazor.Client.GameLogic;

namespace TetrisBlazor.Tests.Client;

public class TetrominoTests
{
    // --- Basic properties ---

    [Theory]
    [InlineData(TetrominoType.I, "#00f0f0")]
    [InlineData(TetrominoType.O, "#f0f000")]
    [InlineData(TetrominoType.T, "#a000f0")]
    [InlineData(TetrominoType.S, "#00f000")]
    [InlineData(TetrominoType.Z, "#f00000")]
    [InlineData(TetrominoType.J, "#0000f0")]
    [InlineData(TetrominoType.L, "#f0a000")]
    public void Constructor_SetsCorrectColor(TetrominoType type, string expectedColor)
    {
        var t = new Tetromino(type);
        Assert.Equal(expectedColor, t.Color);
    }

    [Fact]
    public void Constructor_ShapeIsNotNull()
    {
        var t = new Tetromino(TetrominoType.T);
        Assert.NotNull(t.Shape);
    }

    [Fact]
    public void Constructor_TypeIsSet()
    {
        var t = new Tetromino(TetrominoType.S);
        Assert.Equal(TetrominoType.S, t.Type);
    }

    // --- RotateCW ---

    [Fact]
    public void RotateCW_TShape_ChangesShape()
    {
        var t = new Tetromino(TetrominoType.T);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW();
        Assert.False(ShapesEqual(originalShape, t.Shape));
    }

    [Fact]
    public void RotateCW_4Times_RestoresOriginal()
    {
        var t = new Tetromino(TetrominoType.T);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW();
        t.RotateCW();
        t.RotateCW();
        t.RotateCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    [Fact]
    public void RotateCW_OPiece_ShapeUnchanged()
    {
        // O piece is symmetric – looks the same after rotation
        var t = new Tetromino(TetrominoType.O);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    [Theory]
    [InlineData(TetrominoType.I)]
    [InlineData(TetrominoType.T)]
    [InlineData(TetrominoType.S)]
    [InlineData(TetrominoType.Z)]
    [InlineData(TetrominoType.J)]
    [InlineData(TetrominoType.L)]
    public void RotateCW_4Times_AllTypes_RestoreOriginal(TetrominoType type)
    {
        var t = new Tetromino(type);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW(); t.RotateCW(); t.RotateCW(); t.RotateCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    // --- RotateCCW ---

    [Fact]
    public void RotateCCW_ThenCW_RestoresOriginal()
    {
        var t = new Tetromino(TetrominoType.T);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCCW();
        t.RotateCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    [Fact]
    public void RotateCW_ThenCCW_RestoresOriginal()
    {
        var t = new Tetromino(TetrominoType.S);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW();
        t.RotateCCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    // --- GetRotatedCW / GetRotatedCCW (non-mutating) ---

    [Fact]
    public void GetRotatedCW_DoesNotMutateOriginal()
    {
        var t = new Tetromino(TetrominoType.T);
        var originalShape = (int[,])t.Shape.Clone();
        _ = t.GetRotatedCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    [Fact]
    public void GetRotatedCCW_DoesNotMutateOriginal()
    {
        var t = new Tetromino(TetrominoType.T);
        var originalShape = (int[,])t.Shape.Clone();
        _ = t.GetRotatedCCW();
        Assert.True(ShapesEqual(originalShape, t.Shape));
    }

    [Fact]
    public void GetRotatedCW_ResultMatchesMutatingRotate()
    {
        var t1 = new Tetromino(TetrominoType.J);
        var t2 = new Tetromino(TetrominoType.J);
        var nonMutating = t1.GetRotatedCW();
        t2.RotateCW();
        Assert.True(ShapesEqual(nonMutating, t2.Shape));
    }

    // --- Clone ---

    [Fact]
    public void Clone_ProducesEqualShape()
    {
        var t = new Tetromino(TetrominoType.L);
        var clone = t.Clone();
        Assert.True(ShapesEqual(t.Shape, clone.Shape));
        Assert.Equal(t.Color, clone.Color);
        Assert.Equal(t.Type, clone.Type);
    }

    [Fact]
    public void Clone_IsIndependent()
    {
        var t = new Tetromino(TetrominoType.L);
        var clone = t.Clone();
        t.RotateCW();
        Assert.False(ShapesEqual(t.Shape, clone.Shape));
    }

    // --- Random ---

    [Fact]
    public void Random_ReturnsNonNull()
    {
        var t = Tetromino.Random();
        Assert.NotNull(t);
    }

    [Fact]
    public void Random_ReturnsValidType()
    {
        var t = Tetromino.Random();
        Assert.True(Enum.IsDefined(t.Type));
    }

    [Fact]
    public void Random_ProducesVariety()
    {
        // With 7 pieces and 50 attempts, chance of all same ≈ 0
        var types = new HashSet<TetrominoType>();
        for (int i = 0; i < 50; i++) types.Add(Tetromino.Random().Type);
        Assert.True(types.Count > 1);
    }

    [Fact]
    public void Random_ShapeIsNotNull()
    {
        var t = Tetromino.Random();
        Assert.NotNull(t.Shape);
        Assert.True(t.Shape.GetLength(0) > 0);
        Assert.True(t.Shape.GetLength(1) > 0);
    }

    // --- Helper ---

    private static bool ShapesEqual(int[,] a, int[,] b)
    {
        if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1)) return false;
        for (int r = 0; r < a.GetLength(0); r++)
        for (int c = 0; c < a.GetLength(1); c++)
            if (a[r, c] != b[r, c]) return false;
        return true;
    }
}
