# Tetris

Webová hra Tetris ovládaná klávesnicí s online leaderboardem.

## Spuštění

### Frontend (hra)
Otevři `index.html` přímo v prohlížeči – hra funguje i bez backendu.

### Backend (high scores API)
```bash
npm install
node server.js
```
Server běží na `http://localhost:3000`.

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
index.html          # Hlavní stránka hry
style.css           # Styly
js/
  tetromino.js      # Definice a rotace tetrominů
  board.js          # Herní pole, kolize, mazání řádků
  renderer.js       # Canvas vykreslování
  input.js          # Klávesové ovládání s DAS
  score.js          # Skórování a levely
  ui.js             # Overlay obrazovky (start, pauza, game over)
  game.js           # Hlavní herní smyčka
server.js           # Express REST API
scores.json         # Perzistentní úložiště skóre
```
