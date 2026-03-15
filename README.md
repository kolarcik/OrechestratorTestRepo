# Tetris – Blazor .NET

Webová hra Tetris v Blazor WebAssembly ovládaná klávesnicí s online leaderboardem.

## Architektura

```
TetrisBlazor.slnx
├── TetrisBlazor.Server/    # ASP.NET Core – hostuje hru + Scores REST API
├── TetrisBlazor.Client/    # Blazor WebAssembly – herní logika + UI
└── TetrisBlazor.Shared/    # Sdílené modely (ScoreEntry, ScoreSubmission)
```

## Spuštění

```bash
cd TetrisBlazor.Server
dotnet run
```

Hra běží na `http://localhost:5XXX` (viz výstup konzole).

## Ovládání

| Klávesa | Akce |
|---|---|
| ← / → | Pohyb doleva / doprava |
| ↑ nebo X | Rotace po směru hodinových ručiček |
| Z | Rotace proti směru hodinových ručiček |
| ↓ | Soft drop |
| Mezerník | Hard drop |
| P | Pauza / Resume |
| Enter | Start / Restart |

## API

| Endpoint | Popis |
|---|---|
| `GET /api/scores` | Top 10 high scores |
| `POST /api/scores` | Uložení nového skóre |

### POST `/api/scores`
```json
{ "name": "string", "score": 1500, "level": 3, "lines": 15 }
```

## Struktura projektu

```
TetrisBlazor.Client/
  GameLogic/
    Tetromino.cs        # 7 tetrominů, rotace CW/CCW
    Board.cs            # 10×20 pole, kolize, mazání řádků, ghost piece
    ScoreManager.cs     # Skóre, level, rychlost
    GameState.cs        # Herní stav (Idle/Playing/Paused/GameOver)
  Components/
    TetrisGame.razor    # Hlavní komponenta, game loop (PeriodicTimer)
    GameBoard.razor     # CSS Grid 10×20, ghost piece
    Sidebar.razor       # Next piece, skóre, leaderboard
    GameOverlay.razor   # Start / Pauza / Game Over overlay
  wwwroot/
    js/tetrisInterop.js # JS interop pro globální keyboard handling
    css/tetris.css      # Dark theme, CSS Grid herní pole

TetrisBlazor.Server/
  Program.cs            # Minimal API + hostování Blazor WASM
```

