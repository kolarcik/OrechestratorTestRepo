using TetrisBlazor.Client.GameLogic;

namespace TetrisBlazor.Tests.Client;

public class BoardTests
{
    private static int[,] SingleBlock => new int[,] { { 1 } };
    private static int[,] IHorizontal => new int[,] { { 1, 1, 1, 1 } };
    private static int[,] OPiece => new int[,] { { 1, 1 }, { 1, 1 } };
    private static int[,] FullRow => new int[,] { { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 } };

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
        var exception = Record.Exception(() => board.Lock(OPiece, 0, -1, "#fff"));
        Assert.Null(exception);
        // Bottom row of piece lands on row 0
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
        board.Lock(FullRow, 0, 19, "#f00");
        Assert.Equal(1, board.ClearLines());
    }

    [Fact]
    public void ClearLines_OneLine_CellIsCleared()
    {
        var board = new Board();
        board.Lock(FullRow, 0, 19, "#f00");
        board.ClearLines();
        Assert.Null(board.GetCell(19, 0));
    }

    [Fact]
    public void ClearLines_TwoLines_Returns2()
    {
        var board = new Board();
        board.Lock(FullRow, 0, 18, "#f00");
        board.Lock(FullRow, 0, 19, "#f00");
        Assert.Equal(2, board.ClearLines());
    }

    [Fact]
    public void ClearLines_Tetris_Returns4()
    {
        var board = new Board();
        for (int r = 16; r <= 19; r++)
            board.Lock(FullRow, 0, r, "#f00");
        Assert.Equal(4, board.ClearLines());
    }

    [Fact]
    public void ClearLines_BlockAboveDrops()
    {
        var board = new Board();
        // Marker at row 18, full line at row 19
        board.Lock(SingleBlock, 0, 18, "#abc");
        board.Lock(FullRow, 0, 19, "#f00");
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
        Assert.Null(board.GetCell(1, 0));
        Assert.Null(board.GetCell(1, 1));
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
