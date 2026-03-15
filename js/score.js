const LINE_POINTS = [0, 100, 300, 500, 800];
class ScoreManager {
  constructor() { this.reset(); }
  reset() { this.score = 0; this.level = 1; this.lines = 0; }
  addLines(count) {
    this.score += (LINE_POINTS[count] || 0) * this.level;
    this.lines += count;
    this.level = Math.floor(this.lines / 10) + 1;
  }
  addSoftDrop(cells) { this.score += cells; }
  addHardDrop(cells) { this.score += cells * 2; }
  getSpeed() { return Math.max(100, 1000 - (this.level - 1) * 100); }
  getScore() { return this.score; }
  getLevel() { return this.level; }
  getLines() { return this.lines; }
}
