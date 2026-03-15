class UIManager {
  constructor() {
    this.overlay = document.getElementById('overlay');
    this.overlayTitle = document.getElementById('overlay-title');
    this.overlayContent = document.getElementById('overlay-content');
    this.scoreDisplay = document.getElementById('score');
    this.levelDisplay = document.getElementById('level');
    this.linesDisplay = document.getElementById('lines');
    this.leaderboardEl = document.getElementById('leaderboard');
  }
  showStart(leaderboard) {
    this.overlayTitle.textContent = 'TETRIS';
    let html = '<p>Press ENTER to start</p>';
    html += '<div class="controls-hint"><small>← → Move &nbsp; ↑/X Rotate CW &nbsp; Z Rotate CCW<br>↓ Soft Drop &nbsp; SPACE Hard Drop &nbsp; P Pause</small></div>';
    if (leaderboard.length > 0) {
      html += '<div class="lb-overlay"><h3>TOP SCORES</h3><ol>';
      leaderboard.slice(0,5).forEach(s => { html += `<li>${s.name} &ndash; ${s.score}</li>`; });
      html += '</ol></div>';
    }
    this.overlayContent.innerHTML = html;
    this.overlay.style.display = 'flex';
  }
  showPause() {
    this.overlayTitle.textContent = 'PAUSED';
    this.overlayContent.innerHTML = '<p>Press P to resume</p>';
    this.overlay.style.display = 'flex';
  }
  showGameOver(score, level, lines, onSubmit) {
    this.overlayTitle.textContent = 'GAME OVER';
    this.overlayContent.innerHTML = `
      <p>Score: ${score}</p>
      <p>Level: ${level} | Lines: ${lines}</p>
      <div class="submit-score">
        <input type="text" id="player-name" maxlength="20" placeholder="Your name" />
        <button id="submit-score-btn">Submit Score</button>
      </div>
      <p><small>Press ENTER to play again</small></p>`;
    this.overlay.style.display = 'flex';
    const btn = document.getElementById('submit-score-btn');
    const input = document.getElementById('player-name');
    const submit = () => {
      if (input.value.trim()) {
        onSubmit(input.value.trim());
        btn.disabled = true;
        btn.textContent = 'Saved!';
      }
    };
    btn.addEventListener('click', submit);
    input.addEventListener('keydown', e => { if (e.code === 'Enter') { e.stopPropagation(); submit(); } });
  }
  hideOverlay() { this.overlay.style.display = 'none'; }
  updateScore(score, level, lines) {
    this.scoreDisplay.textContent = score;
    this.levelDisplay.textContent = level;
    this.linesDisplay.textContent = lines;
  }
  updateLeaderboard(leaderboard) {
    if (!this.leaderboardEl) return;
    this.leaderboardEl.innerHTML = leaderboard.slice(0,10).map(
      (s,i) => `<li>${i+1}. ${s.name} <span>${s.score}</span></li>`
    ).join('');
  }
}
