# Úkol: Frontend – Tetris Web Game

## Přehled
Implementuj kompletní hru Tetris jako single-page webovou aplikaci ovládanou klávesnicí.

## Technologie
- HTML5 + CSS3
- Vanilla JavaScript (bez frameworků)
- HTML5 Canvas pro vykreslování
- Fetch API pro komunikaci s backendem

## Struktura souborů – vytvoř přesně tuto strukturu:
```
index.html
style.css
js/
  tetromino.js   # definice tetrominů
  board.js       # herní pole + kolize
  renderer.js    # Canvas vykreslování
  input.js       # keyboard handling
  score.js       # skórování + backend volání
  ui.js          # overlay obrazovky
  game.js        # hlavní smyčka + inicializace
```

## Detailní specifikace

### index.html
- Canvas element (300px šířka = 10 sloupců × 30px)
- Sidebar vpravo: next piece canvas, score/level/lines display, leaderboard seznam
- Overlay div pro start/pause/gameover obrazovky
- Načíst všechny JS soubory v pořadí: tetromino → board → renderer → input → score → ui → game

### style.css
- Tmavé pozadí (#1a1a2e nebo podobné)
- Centered layout, canvas s border
- Pěkný retro/gaming look (doporučuji Google Font "Press Start 2P" nebo podobný)
- Responsivní: hra se vejde na obrazovku

### js/tetromino.js
Definuj 7 tetrominů jako objekty s vlastnostmi `shape` (2D pole) a `color`:
- **I** – azurová (#00f0f0)
- **O** – žlutá (#f0f000)
- **T** – fialová (#a000f0)
- **S** – zelená (#00f000)
- **Z** – červená (#f00000)
- **J** – modrá (#0000f0)
- **L** – oranžová (#f0a000)

Funkce `rotateCW(shape)` – rotace matice po směru hodinových ručiček.
Funkce `rotateCCW(shape)` – rotace matice proti směru.
Funkce `randomTetromino()` – vrátí náhodný tetromino objekt (kopii).

### js/board.js
Třída `Board`:
- `constructor()`: vytvoří 20×10 mřížku (pole polí), inicializuje ji nulami
- `isValid(shape, pos)`: vrátí true pokud je pozice platná (v hranicích, bez kolize)
- `lock(shape, pos, color)`: zapíše tetromino do mřížky
- `clearLines()`: smaže plné řádky, vrátí počet smazaných řádků
- `getGrid()`: vrátí mřížku
- `reset()`: vymaže mřížku

### js/renderer.js
Třída `Renderer`:
- `constructor(canvas, nextCanvas)`: inicializace Canvas kontextů
- `drawBoard(board)`: vykreslí mřížku (zamrzlé bloky + mřížkové čáry)
- `drawPiece(shape, pos, color)`: vykreslí aktivní tetromino
- `drawGhost(shape, pos, color, board)`: vykreslí ghost piece (průhledně, stejná X pozice, spadlá na dno)
- `drawNext(tetromino)`: vykreslí next piece v sidebaru
- `clear()`: vymaže hlavní canvas

Konstanta `BLOCK_SIZE = 30` (px).

### js/input.js
Třída `InputHandler`:
- `constructor(callbacks)`: callbacks = `{ moveLeft, moveRight, softDrop, hardDrop, rotateCW, rotateCCW, pause }`
- Registruje keydown event listener
- Mapování:
  - ArrowLeft → moveLeft
  - ArrowRight → moveRight
  - ArrowDown → softDrop
  - ArrowUp / KeyX → rotateCW
  - KeyZ → rotateCCW
  - Space → hardDrop
  - KeyP → pause
- `preventDefault()` pro všechny herní klávesy (zabrání scrollování)
- DAS (Delayed Auto Shift): pro ArrowLeft/ArrowRight implementuj auto-repeat (delay 150ms, repeat 50ms)

### js/score.js
Třída `ScoreManager`:
- `constructor()`: score=0, level=1, lines=0
- `addLines(count)`: přidá body dle tabulky (×level), zvýší lines, přepočítá level (každých 10 řádků)
  - 1 řádek: 100 × level
  - 2 řádky: 300 × level
  - 3 řádky: 500 × level
  - 4 řádky (Tetris): 800 × level
- `addSoftDrop(cells)`: +1 bod × cells
- `addHardDrop(cells)`: +2 body × cells
- `getSpeed()`: vrátí interval gravity v ms (1000 - (level-1)*100, minimum 100ms)
- `reset()`: vynuluje vše
- Gettery: `getScore()`, `getLevel()`, `getLines()`

### js/ui.js
Třída `UIManager`:
- `constructor()`: reference na DOM elementy
- `showStart(leaderboard)`: zobrazí start screen s leaderboardem (top 10)
- `showPause()`: zobrazí pause overlay
- `hidePause()`: skryje pause overlay
- `showGameOver(score, onSubmit)`: zobrazí game over screen se skóre, input pro jméno (max 20 znaků), submit tlačítko; volá `onSubmit(name)` po kliknutí/enteru
- `hideOverlay()`: skryje všechny overlaye
- `updateScore(score, level, lines)`: aktualizuje sidebar

### js/game.js
Hlavní orchestrátor:
- `init()`: vytvoří instance všech tříd, načte leaderboard z backendu, zobrazí start screen
- `start()`: reset board/score, spawnuje první tetromino, spustí herní smyčku
- `gameLoop(timestamp)`: requestAnimationFrame, řídí gravity timer
- `spawnPiece()`: umístí nové tetromino nahoře; pokud není validní → game over
- `lockPiece()`: zavolá board.lock(), board.clearLines(), score.addLines(), spawnuje nové tetromino
- Gravity: automatický pád každých `scoreManager.getSpeed()` ms
- Stavový automat: `idle | playing | paused | gameover`

### Backend integrace (v game.js nebo score.js)
```javascript
const API_URL = 'http://localhost:3000/api';

async function loadLeaderboard() {
  try {
    const res = await fetch(`${API_URL}/scores`);
    return await res.json();
  } catch { return []; }  // fallback: prázdný leaderboard
}

async function submitScore(name, score, level, lines) {
  try {
    await fetch(`${API_URL}/scores`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, score, level, lines })
    });
  } catch { /* ignoruj chybu */ }
}
```

## Důležité
- Hra musí být hratelná i bez backendu (graceful degradation)
- Otevřít přímo `index.html` v prohlížeči musí fungovat
- Testuj v Chrome/Firefox
