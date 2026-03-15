const BLOCK_SIZE = 30;

class Renderer {
  constructor(canvas, nextCanvas) {
    this.canvas = canvas;
    this.ctx = canvas.getContext('2d');
    this.nextCanvas = nextCanvas;
    this.nextCtx = nextCanvas.getContext('2d');
  }

  clear() {
    this.ctx.fillStyle = '#0d0d1a';
    this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
  }

  drawBoard(board) {
    const grid = board.getGrid();
    for (let r = 0; r < grid.length; r++) {
      for (let c = 0; c < grid[r].length; c++) {
        if (grid[r][c]) {
          this._drawBlock(this.ctx, c, r, grid[r][c]);
        } else {
          this._drawEmpty(this.ctx, c, r);
        }
      }
    }
    this._drawGrid();
  }

  drawPiece(shape, pos, color) {
    for (let r = 0; r < shape.length; r++) {
      for (let c = 0; c < shape[r].length; c++) {
        if (shape[r][c]) {
          this._drawBlock(this.ctx, pos.x + c, pos.y + r, color);
        }
      }
    }
  }

  drawGhost(shape, pos, color, board) {
    let ghostY = pos.y;
    while (board.isValid(shape, { x: pos.x, y: ghostY + 1 })) {
      ghostY++;
    }
    if (ghostY === pos.y) return;
    for (let r = 0; r < shape.length; r++) {
      for (let c = 0; c < shape[r].length; c++) {
        if (shape[r][c]) {
          this._drawGhostBlock(this.ctx, pos.x + c, ghostY + r, color);
        }
      }
    }
  }

  drawNext(tetromino) {
    const { shape, color } = tetromino;
    this.nextCtx.fillStyle = '#0d0d1a';
    this.nextCtx.fillRect(0, 0, this.nextCanvas.width, this.nextCanvas.height);
    const offsetX = Math.floor((4 - shape[0].length) / 2);
    const offsetY = Math.floor((4 - shape.length) / 2);
    for (let r = 0; r < shape.length; r++) {
      for (let c = 0; c < shape[r].length; c++) {
        if (shape[r][c]) {
          this._drawBlock(this.nextCtx, offsetX + c, offsetY + r, color, BLOCK_SIZE - 2);
        }
      }
    }
  }

  _drawBlock(ctx, col, row, color, size = BLOCK_SIZE) {
    const x = col * size;
    const y = row * size;
    ctx.fillStyle = color;
    ctx.fillRect(x + 1, y + 1, size - 2, size - 2);
    ctx.fillStyle = 'rgba(255,255,255,0.25)';
    ctx.fillRect(x + 1, y + 1, size - 2, 4);
    ctx.fillStyle = 'rgba(0,0,0,0.3)';
    ctx.fillRect(x + 1, y + size - 5, size - 2, 4);
  }

  _drawEmpty(ctx, col, row) {
    const x = col * BLOCK_SIZE;
    const y = row * BLOCK_SIZE;
    ctx.fillStyle = '#0d0d1a';
    ctx.fillRect(x, y, BLOCK_SIZE, BLOCK_SIZE);
  }

  _drawGhostBlock(ctx, col, row, color) {
    const x = col * BLOCK_SIZE;
    const y = row * BLOCK_SIZE;
    ctx.strokeStyle = color;
    ctx.globalAlpha = 0.35;
    ctx.strokeRect(x + 1, y + 1, BLOCK_SIZE - 2, BLOCK_SIZE - 2);
    ctx.globalAlpha = 1;
  }

  _drawGrid() {
    this.ctx.strokeStyle = 'rgba(255,255,255,0.04)';
    this.ctx.lineWidth = 0.5;
    for (let c = 0; c <= 10; c++) {
      this.ctx.beginPath();
      this.ctx.moveTo(c * BLOCK_SIZE, 0);
      this.ctx.lineTo(c * BLOCK_SIZE, 20 * BLOCK_SIZE);
      this.ctx.stroke();
    }
    for (let r = 0; r <= 20; r++) {
      this.ctx.beginPath();
      this.ctx.moveTo(0, r * BLOCK_SIZE);
      this.ctx.lineTo(10 * BLOCK_SIZE, r * BLOCK_SIZE);
      this.ctx.stroke();
    }
  }
}
