const API_URL = 'http://localhost:3000/api';

async function loadLeaderboard() {
  try {
    const res = await fetch(`${API_URL}/scores`);
    return await res.json();
  } catch { return []; }
}

async function submitScore(name, score, level, lines) {
  try {
    await fetch(`${API_URL}/scores`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ name, score, level, lines })
    });
  } catch { /* backend not available */ }
}

class Game {
  constructor() {
    this.board = new Board();
    this.scoreManager = new ScoreManager();
    this.renderer = new Renderer(
      document.getElementById('game-canvas'),
      document.getElementById('next-canvas')
    );
    this.ui = new UIManager();
    this.state = 'idle';
    this.currentPiece = null;
    this.currentPos = null;
    this.nextPiece = null;
    this.lastTime = 0;
    this.dropCounter = 0;
    this.input = new InputHandler({
      moveLeft: () => this.move(-1),
      moveRight: () => this.move(1),
      softDrop: () => this.softDrop(),
      hardDrop: () => this.hardDrop(),
      rotateCW: () => this.rotate('cw'),
      rotateCCW: () => this.rotate('ccw'),
      pause: () => this.togglePause()
    });
    document.addEventListener('keydown', e => {
      if (e.code === 'Enter') {
        if (this.state === 'idle' || this.state === 'gameover') this.start();
      }
    });
  }

  async init() {
    const leaderboard = await loadLeaderboard();
    this.ui.showStart(leaderboard);
    this.ui.updateLeaderboard(leaderboard);
  }

  start() {
    this.board.reset();
    this.scoreManager.reset();
    this.nextPiece = randomTetromino();
    this.state = 'playing';
    this.ui.hideOverlay();
    this.spawnPiece();
    this.lastTime = 0;
    this.dropCounter = 0;
    requestAnimationFrame(ts => this.gameLoop(ts));
  }

  spawnPiece() {
    this.currentPiece = this.nextPiece;
    this.nextPiece = randomTetromino();
    this.currentPos = { x: Math.floor(COLS / 2) - Math.floor(this.currentPiece.shape[0].length / 2), y: 0 };
    if (!this.board.isValid(this.currentPiece.shape, this.currentPos)) {
      this.state = 'gameover';
      this.ui.showGameOver(
        this.scoreManager.getScore(),
        this.scoreManager.getLevel(),
        this.scoreManager.getLines(),
        async (name) => {
          await submitScore(name, this.scoreManager.getScore(), this.scoreManager.getLevel(), this.scoreManager.getLines());
          const lb = await loadLeaderboard();
          this.ui.updateLeaderboard(lb);
        }
      );
    }
    this.renderer.drawNext(this.nextPiece);
  }

  gameLoop(timestamp) {
    if (this.state !== 'playing') return;
    if (this.lastTime === 0) this.lastTime = timestamp;
    const delta = timestamp - this.lastTime;
    this.lastTime = timestamp;
    this.dropCounter += delta;
    if (this.dropCounter >= this.scoreManager.getSpeed()) {
      this.dropCounter = 0;
      this.gravity();
    }
    this.render();
    requestAnimationFrame(ts => this.gameLoop(ts));
  }

  gravity() {
    const newPos = { x: this.currentPos.x, y: this.currentPos.y + 1 };
    if (this.board.isValid(this.currentPiece.shape, newPos)) {
      this.currentPos = newPos;
    } else {
      this.lockPiece();
    }
  }

  lockPiece() {
    this.board.lock(this.currentPiece.shape, this.currentPos, this.currentPiece.color);
    const cleared = this.board.clearLines();
    if (cleared > 0) this.scoreManager.addLines(cleared);
    this.ui.updateScore(this.scoreManager.getScore(), this.scoreManager.getLevel(), this.scoreManager.getLines());
    this.spawnPiece();
  }

  move(dir) {
    if (this.state !== 'playing') return;
    const newPos = { x: this.currentPos.x + dir, y: this.currentPos.y };
    if (this.board.isValid(this.currentPiece.shape, newPos)) this.currentPos = newPos;
  }

  softDrop() {
    if (this.state !== 'playing') return;
    const newPos = { x: this.currentPos.x, y: this.currentPos.y + 1 };
    if (this.board.isValid(this.currentPiece.shape, newPos)) {
      this.currentPos = newPos;
      this.scoreManager.addSoftDrop(1);
      this.dropCounter = 0;
    } else {
      this.lockPiece();
    }
  }

  hardDrop() {
    if (this.state !== 'playing') return;
    let cells = 0;
    while (this.board.isValid(this.currentPiece.shape, { x: this.currentPos.x, y: this.currentPos.y + 1 })) {
      this.currentPos.y++;
      cells++;
    }
    this.scoreManager.addHardDrop(cells);
    this.lockPiece();
  }

  rotate(dir) {
    if (this.state !== 'playing') return;
    const rotated = dir === 'cw' ? rotateCW(this.currentPiece.shape) : rotateCCW(this.currentPiece.shape);
    // Wall kick: try original pos, then ±1, ±2
    for (const kick of [0, -1, 1, -2, 2]) {
      const newPos = { x: this.currentPos.x + kick, y: this.currentPos.y };
      if (this.board.isValid(rotated, newPos)) {
        this.currentPiece.shape = rotated;
        this.currentPos = newPos;
        return;
      }
    }
  }

  togglePause() {
    if (this.state === 'playing') {
      this.state = 'paused';
      this.ui.showPause();
    } else if (this.state === 'paused') {
      this.state = 'playing';
      this.ui.hideOverlay();
      this.lastTime = 0;
      requestAnimationFrame(ts => this.gameLoop(ts));
    }
  }

  render() {
    this.renderer.clear();
    this.renderer.drawBoard(this.board);
    if (this.currentPiece) {
      this.renderer.drawGhost(this.currentPiece.shape, this.currentPos, this.currentPiece.color, this.board);
      this.renderer.drawPiece(this.currentPiece.shape, this.currentPos, this.currentPiece.color);
    }
  }
}

const game = new Game();
game.init();
