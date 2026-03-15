class Board {
  constructor() {
    this.rows = 20;
    this.cols = 10;
    this.grid = [];
    this.reset();
  }

  reset() {
    this.grid = Array.from({ length: this.rows }, () => Array(this.cols).fill(0));
  }

  getGrid() {
    return this.grid;
  }

  isValid(shape, pos) {
    for (let r = 0; r < shape.length; r++) {
      for (let c = 0; c < shape[r].length; c++) {
        if (!shape[r][c]) continue;
        const newR = pos.y + r;
        const newC = pos.x + c;
        if (newC < 0 || newC >= this.cols) return false;
        if (newR >= this.rows) return false;
        if (newR < 0) continue;
        if (this.grid[newR][newC]) return false;
      }
    }
    return true;
  }

  lock(shape, pos, color) {
    for (let r = 0; r < shape.length; r++) {
      for (let c = 0; c < shape[r].length; c++) {
        if (!shape[r][c]) continue;
        const newR = pos.y + r;
        const newC = pos.x + c;
        if (newR >= 0 && newR < this.rows && newC >= 0 && newC < this.cols) {
          this.grid[newR][newC] = color;
        }
      }
    }
  }

  clearLines() {
    let cleared = 0;
    for (let r = this.rows - 1; r >= 0; r--) {
      if (this.grid[r].every(cell => cell !== 0)) {
        this.grid.splice(r, 1);
        this.grid.unshift(Array(this.cols).fill(0));
        cleared++;
        r++; // re-check same row index after splice
      }
    }
    return cleared;
  }
}
