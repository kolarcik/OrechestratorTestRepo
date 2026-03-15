using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TetrisBlazor.Client.GameLogic;
using TetrisBlazor.Shared;

namespace TetrisBlazor.Client.Components;

public partial class TetrisGame : IAsyncDisposable
{
    private readonly GameState _state = new();
    private List<ScoreEntry> _leaderboard = [];
    private PeriodicTimer? _timer;
    private DotNetObjectReference<TetrisGame>? _dotNetRef;
    private bool _scoreSubmitted;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;
        _dotNetRef = DotNetObjectReference.Create(this);
        await JS.InvokeVoidAsync("tetrisInterop.init", _dotNetRef);
        await LoadLeaderboard();
        StateHasChanged();
    }

    private async Task LoadLeaderboard()
    {
        try { _leaderboard = await Http.GetFromJsonAsync<List<ScoreEntry>>("api/scores") ?? []; }
        catch { _leaderboard = []; }
    }

    [JSInvokable]
    public void HandleKey(string code)
    {
        if (_state.Status is GameStatus.Idle or GameStatus.GameOver)
        {
            if (code == "Enter") { _ = StartGame(); return; }
        }
        if (_state.Status is GameStatus.Playing or GameStatus.Paused)
        {
            if (code == "KeyP") { TogglePause(); return; }
        }
        if (_state.Status != GameStatus.Playing) return;

        switch (code)
        {
            case "ArrowLeft":  TryMove(-1, 0); break;
            case "ArrowRight": TryMove(1, 0); break;
            case "ArrowDown":  SoftDrop(); break;
            case "ArrowUp":
            case "KeyX":       TryRotate(cw: true); break;
            case "KeyZ":       TryRotate(cw: false); break;
            case "Space":      HardDrop(); break;
        }
        InvokeAsync(StateHasChanged);
    }

    private async Task StartGame()
    {
        _timer?.Dispose();
        _state.Reset();
        _scoreSubmitted = false;
        _state.NextPiece = Tetromino.Random();
        _state.Status = GameStatus.Playing;
        SpawnPiece();
        await InvokeAsync(StateHasChanged);
        await RunGameLoop();
    }

    private async Task RunGameLoop()
    {
        _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_state.ScoreManager.GetSpeedMs()));
        try
        {
            while (await _timer.WaitForNextTickAsync())
            {
                if (_state.Status != GameStatus.Playing) continue;
                Gravity();
                var newSpeed = TimeSpan.FromMilliseconds(_state.ScoreManager.GetSpeedMs());
                if (_timer.Period != newSpeed)
                    _timer.Period = newSpeed;
                await InvokeAsync(StateHasChanged);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void SpawnPiece()
    {
        _state.CurrentPiece = _state.NextPiece ?? Tetromino.Random();
        _state.NextPiece = Tetromino.Random();
        _state.CurrentX = Board.Cols / 2 - _state.CurrentPiece.Shape.GetLength(1) / 2;
        _state.CurrentY = 0;

        if (!_state.Board.IsValid(_state.CurrentPiece.Shape, _state.CurrentX, _state.CurrentY))
        {
            _state.Status = GameStatus.GameOver;
            _timer?.Dispose();
        }
    }

    private void Gravity()
    {
        if (_state.CurrentPiece is null) return;
        if (_state.Board.IsValid(_state.CurrentPiece.Shape, _state.CurrentX, _state.CurrentY + 1))
            _state.CurrentY++;
        else
            LockPiece();
    }

    private void LockPiece()
    {
        if (_state.CurrentPiece is null) return;
        _state.Board.Lock(_state.CurrentPiece.Shape, _state.CurrentX, _state.CurrentY, _state.CurrentPiece.Color);
        var cleared = _state.Board.ClearLines();
        if (cleared > 0) _state.ScoreManager.AddLines(cleared);
        SpawnPiece();
    }

    private void TryMove(int dx, int dy)
    {
        if (_state.CurrentPiece is null) return;
        if (_state.Board.IsValid(_state.CurrentPiece.Shape, _state.CurrentX + dx, _state.CurrentY + dy))
        {
            _state.CurrentX += dx;
            _state.CurrentY += dy;
        }
    }

    private void TryRotate(bool cw)
    {
        if (_state.CurrentPiece is null) return;
        var rotated = cw ? _state.CurrentPiece.GetRotatedCW() : _state.CurrentPiece.GetRotatedCCW();
        foreach (var kick in new[] { 0, -1, 1, -2, 2 })
        {
            if (_state.Board.IsValid(rotated, _state.CurrentX + kick, _state.CurrentY))
            {
                if (cw) _state.CurrentPiece.RotateCW(); else _state.CurrentPiece.RotateCCW();
                _state.CurrentX += kick;
                return;
            }
        }
    }

    private void SoftDrop()
    {
        if (_state.CurrentPiece is null) return;
        if (_state.Board.IsValid(_state.CurrentPiece.Shape, _state.CurrentX, _state.CurrentY + 1))
        {
            _state.CurrentY++;
            _state.ScoreManager.AddSoftDrop(1);
        }
        else LockPiece();
    }

    private void HardDrop()
    {
        if (_state.CurrentPiece is null) return;
        int cells = 0;
        while (_state.Board.IsValid(_state.CurrentPiece.Shape, _state.CurrentX, _state.CurrentY + 1))
        { _state.CurrentY++; cells++; }
        _state.ScoreManager.AddHardDrop(cells);
        LockPiece();
    }

    private void TogglePause()
    {
        _state.Status = _state.Status == GameStatus.Playing ? GameStatus.Paused : GameStatus.Playing;
        InvokeAsync(StateHasChanged);
    }

    private async Task SubmitScore(string name)
    {
        if (_scoreSubmitted) return;
        _scoreSubmitted = true;
        try
        {
            var submission = new ScoreSubmission
            {
                Name = name,
                Score = _state.ScoreManager.Score,
                Level = _state.ScoreManager.Level,
                Lines = _state.ScoreManager.Lines
            };
            await Http.PostAsJsonAsync("api/scores", submission);
            await LoadLeaderboard();
            await InvokeAsync(StateHasChanged);
        }
        catch { /* backend not available */ }
    }

    public async ValueTask DisposeAsync()
    {
        _timer?.Dispose();
        if (_dotNetRef != null)
        {
            try { await JS.InvokeVoidAsync("tetrisInterop.dispose"); } catch { }
            _dotNetRef.Dispose();
        }
    }
}
