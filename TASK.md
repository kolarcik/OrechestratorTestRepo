# Úkol: Frontend – Unit testy pro herní logiku

## Přehled
Vytvoř unit test projekt pro `TetrisBlazor.Client` herní logiku: `Board`, `ScoreManager`, `Tetromino`.
Razor komponenty testovat nebudeme (vyžadují Blazor test framework) – zaměř se na čistou C# logiku.

## Pracovní adresář
```
/Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Frontend
```

**DŮLEŽITÉ:** Solution soubor (`TetrisBlazor.slnx`) vlastní Backend worker. Vytvoř test projekt jako standalone – bez přidávání do slnx. Test projekt bude v adresáři `TetrisBlazor.Tests.Client/`.

## Krok 1: Vytvoř test projekt

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Frontend

dotnet new xunit -n TetrisBlazor.Tests.Client -o TetrisBlazor.Tests.Client --framework net10.0

cd TetrisBlazor.Tests.Client

# Přidej project reference na Client (GameLogic je součástí Client projektu)
dotnet add reference ../TetrisBlazor.Client/TetrisBlazor.Client.csproj
```

## Krok 2: Uprav TetrisBlazor.Tests.Client.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.*" />
    <PackageReference Include="xunit" Version="2.*" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.*">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.*">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\TetrisBlazor.Client\TetrisBlazor.Client.csproj" />
  </ItemGroup>
</Project>
```

**POZOR:** `TetrisBlazor.Client` je `Microsoft.NET.Sdk.BlazorWebAssembly` projekt. Referencing na něj z test projektu může způsobit build problémy kvůli WASM specifickým věcem. Pokud `dotnet build` selže, řešení je přesunout `GameLogic/` do samostatného `TetrisBlazor.GameLogic` class library projektu a referencovat jen ten. Viz záložní plán níže.

### Záložní plán – pokud reference na Client projekt selže:

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Frontend

# Vytvoř samostatnou class library pro herní logiku
dotnet new classlib -n TetrisBlazor.GameLogic -o TetrisBlazor.GameLogic --framework net10.0
rm TetrisBlazor.GameLogic/Class1.cs

# Přesuň GameLogic soubory
cp TetrisBlazor.Client/GameLogic/*.cs TetrisBlazor.GameLogic/

# Uprav namespace v přesunutých souborech pokud je potřeba
# (namespace je 'TetrisBlazor.Client.GameLogic' – ponech stejný, ať nemusíš měnit Client)

# V Client projektu smaž původní soubory a přidej referenci na GameLogic:
# dotnet add TetrisBlazor.Client/TetrisBlazor.Client.csproj reference TetrisBlazor.GameLogic/TetrisBlazor.GameLogic.csproj

# V test projektu referencuj GameLogic:
dotnet add TetrisBlazor.Tests.Client/TetrisBlazor.Tests.Client.csproj reference TetrisBlazor.GameLogic/TetrisBlazor.GameLogic.csproj
```

## Krok 3: Vytvoř test soubory

Smaž výchozí `UnitTest1.cs` a vytvoř tyto soubory:

### `TetrisBlazor.Tests.Client/BoardTests.cs`

```csharp
using TetrisBlazor.Client.GameLogic;

namespace TetrisBlazor.Tests.Client;

public class BoardTests
{
    // Pomocná metoda: jednoduchý 1x1 "blok" shape
    private static int[,] SingleBlock => new int[,] { { 1 } };
    // 1x4 I-piece (horizontální)
    private static int[,] IHorizontal => new int[,] { { 1, 1, 1, 1 } };
    // 2x2 O-piece
    private static int[,] OPiece => new int[,] { { 1, 1 }, { 1, 1 } };

    // --- IsValid ---

    [Fact]
    public void IsValid_EmptyBoard_CenterPosition_ReturnsTrue()
    {
        var board = new Board();
        Assert.True(board.IsValid(SingleBlock, 5, 5));
    }

    [Fact]
    public void IsValid_LeftEdge_ReturnsTrue()
    {
        var board = new Board();
        Assert.True(board.IsValid(SingleBlock, 0, 0));
    }

    [Fact]
    public void IsValid_RightEdge_ReturnsTrue()
    {
        var board = new Board();
        Assert.True(board.IsValid(SingleBlock, Board.Cols - 1, 0));
    }

    [Fact]
    public void IsValid_OutOfBoundsLeft_ReturnsFalse()
    {
        var board = new Board();
        Assert.False(board.IsValid(SingleBlock, -1, 0));
    }

    [Fact]
    public void IsValid_OutOfBoundsRight_ReturnsFalse()
    {
        var board = new Board();
        Assert.False(board.IsValid(SingleBlock, Board.Cols, 0));
    }

    [Fact]
    public void IsValid_OutOfBoundsBottom_ReturnsFalse()
    {
        var board = new Board();
        Assert.False(board.IsValid(SingleBlock, 0, Board.Rows));
    }

    [Fact]
    public void IsValid_AboveBoard_ReturnsTrue()
    {
        // Pieces spawn above the board (negative Y) – must be valid
        var board = new Board();
        Assert.True(board.IsValid(SingleBlock, 5, -1));
    }

    [Fact]
    public void IsValid_OccupiedCell_ReturnsFalse()
    {
        var board = new Board();
        board.Lock(SingleBlock, 5, 5, "#ff0000");
        Assert.False(board.IsValid(SingleBlock, 5, 5));
    }

    [Fact]
    public void IsValid_AdjacentToOccupied_ReturnsTrue()
    {
        var board = new Board();
        board.Lock(SingleBlock, 5, 5, "#ff0000");
        Assert.True(board.IsValid(SingleBlock, 6, 5));
        Assert.True(board.IsValid(SingleBlock, 5, 4));
    }

    // --- Lock ---

    [Fact]
    public void Lock_StoresColor()
    {
        var board = new Board();
        board.Lock(SingleBlock, 3, 7, "#00f0f0");
        Assert.Equal("#00f0f0", board.GetCell(7, 3));
    }

    [Fact]
    public void Lock_MultiCell_StoresAll()
    {
        var board = new Board();
        board.Lock(OPiece, 2, 2, "#f0f000");
        Assert.Equal("#f0f000", board.GetCell(2, 2));
        Assert.Equal("#f0f000", board.GetCell(2, 3));
        Assert.Equal("#f0f000", board.GetCell(3, 2));
        Assert.Equal("#f0f000", board.GetCell(3, 3));
    }

    [Fact]
    public void Lock_PartiallyAboveBoard_DoesNotThrow()
    {
        var board = new Board();
        // Lock at y=-1 (top of piece at row -1, bottom at row 0)
        var exception = Record.Exception(() => board.Lock(OPiece, 0, -1, "#fff"));
        Assert.Null(exception);
        // Row 0, col 0 and 1 should be set
        Assert.Equal("#fff", board.GetCell(0, 0));
        Assert.Equal("#fff", board.GetCell(0, 1));
    }

    // --- ClearLines ---

    [Fact]
    public void ClearLines_NoFullLines_ReturnsZero()
    {
        var board = new Board();
        board.Lock(SingleBlock, 0, 19, "#fff");
        Assert.Equal(0, board.ClearLines());
    }

    [Fact]
    public void ClearLines_OneLine_Returns1()
    {
        var board = new Board();
        // Fill row 19 completely
        var row = new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        board.Lock(row, 0, 19, "#f00");
        Assert.Equal(1, board.ClearLines());
    }

    [Fact]
    public void ClearLines_OneLine_CellIsCleared()
    {
        var board = new Board();
        var row = new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        board.Lock(row, 0, 19, "#f00");
        board.ClearLines();
        Assert.Null(board.GetCell(19, 0));
    }

    [Fact]
    public void ClearLines_TwoLines_Returns2()
    {
        var board = new Board();
        var row = new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        board.Lock(row, 0, 18, "#f00");
        board.Lock(row, 0, 19, "#f00");
        Assert.Equal(2, board.ClearLines());
    }

    [Fact]
    public void ClearLines_Tetris_Returns4()
    {
        var board = new Board();
        var row = new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        for (int r = 16; r <= 19; r++)
            board.Lock(row, 0, r, "#f00");
        Assert.Equal(4, board.ClearLines());
    }

    [Fact]
    public void ClearLines_BlockAboveDrops()
    {
        var board = new Board();
        var row = new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };
        // Put a marker at row 18
        board.Lock(SingleBlock, 0, 18, "#abc");
        // Fill row 19
        board.Lock(row, 0, 19, "#f00");
        board.ClearLines();
        // Marker should have dropped from row 18 to row 19
        Assert.Equal("#abc", board.GetCell(19, 0));
    }

    // --- Reset ---

    [Fact]
    public void Reset_ClearsAllCells()
    {
        var board = new Board();
        board.Lock(OPiece, 0, 0, "#fff");
        board.Reset();
        Assert.Null(board.GetCell(0, 0));
        Assert.Null(board.GetCell(0, 1));
    }

    // --- GetGhostY ---

    [Fact]
    public void GetGhostY_EmptyBoard_FallsToBottom()
    {
        var board = new Board();
        int ghostY = board.GetGhostY(SingleBlock, 5, 0);
        Assert.Equal(Board.Rows - 1, ghostY);
    }

    [Fact]
    public void GetGhostY_BlockedByLockedPiece()
    {
        var board = new Board();
        board.Lock(SingleBlock, 5, 15, "#fff");
        int ghostY = board.GetGhostY(SingleBlock, 5, 0);
        Assert.Equal(14, ghostY);
    }

    [Fact]
    public void GetGhostY_AlreadyAtBottom_ReturnsSameY()
    {
        var board = new Board();
        int ghostY = board.GetGhostY(SingleBlock, 5, Board.Rows - 1);
        Assert.Equal(Board.Rows - 1, ghostY);
    }
}
```

### `TetrisBlazor.Tests.Client/ScoreManagerTests.cs`

```csharp
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
    [InlineData(11, 100)]  // minimum
    [InlineData(20, 100)]  // minimum
    public void GetSpeedMs_DecreasesWithLevel(int level, int expectedMs)
    {
        var sm = new ScoreManager();
        // Advance to desired level
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
```

### `TetrisBlazor.Tests.Client/TetrominoTests.cs`

```csharp
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
        // O piece looks the same after rotation
        var t = new Tetromino(TetrominoType.O);
        var originalShape = (int[,])t.Shape.Clone();
        t.RotateCW();
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
        // With 7 pieces and 50 attempts, chance of all same < (1/7)^49 ≈ 0
        var types = new HashSet<TetrominoType>();
        for (int i = 0; i < 50; i++) types.Add(Tetromino.Random().Type);
        Assert.True(types.Count > 1);
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
```

## Krok 4: Spusť testy

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Frontend
dotnet test TetrisBlazor.Tests.Client/TetrisBlazor.Tests.Client.csproj --logger "console;verbosity=normal"
```

Všechny testy musí projít. Pokud build selže kvůli WASM referenci, použij záložní plán z Kroku 1 (vytvoř `TetrisBlazor.GameLogic` class library).

## Krok 5: Git commit

```bash
cd /Users/ludek.kolarcik/.copilothub/worktrees/TestTeam/Frontend
git add .
git commit -m "test: add unit tests for game logic (Board, ScoreManager, Tetromino)

- BoardTests: IsValid, Lock, ClearLines (1/2/4 lines), GetGhostY, Reset
- ScoreManagerTests: scoring table, level progression, speed, soft/hard drop
- TetrominoTests: colors, rotation CW/CCW, non-mutating getters, Clone, Random

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>"
```

Do NOT push.
