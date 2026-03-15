class InputHandler {
  constructor(callbacks) {
    this.callbacks = callbacks;
    this.dasTimer = null;
    this.dasRepeat = null;
    this.dasDelay = 150;
    this.dasInterval = 50;
    this.gameKeys = new Set([
      'ArrowLeft','ArrowRight','ArrowDown','ArrowUp',
      'KeyX','KeyZ','Space','KeyP'
    ]);
    this._onKeyDown = this._onKeyDown.bind(this);
    this._onKeyUp = this._onKeyUp.bind(this);
    document.addEventListener('keydown', this._onKeyDown);
    document.addEventListener('keyup', this._onKeyUp);
  }

  _onKeyDown(e) {
    if (this.gameKeys.has(e.code)) {
      e.preventDefault();
    }
    switch (e.code) {
      case 'ArrowLeft':
        if (!e.repeat) {
          this.callbacks.moveLeft?.();
          this._startDAS(() => this.callbacks.moveLeft?.());
        }
        break;
      case 'ArrowRight':
        if (!e.repeat) {
          this.callbacks.moveRight?.();
          this._startDAS(() => this.callbacks.moveRight?.());
        }
        break;
      case 'ArrowDown':
        this.callbacks.softDrop?.();
        break;
      case 'ArrowUp':
      case 'KeyX':
        if (!e.repeat) this.callbacks.rotateCW?.();
        break;
      case 'KeyZ':
        if (!e.repeat) this.callbacks.rotateCCW?.();
        break;
      case 'Space':
        if (!e.repeat) this.callbacks.hardDrop?.();
        break;
      case 'KeyP':
        if (!e.repeat) this.callbacks.pause?.();
        break;
    }
  }

  _onKeyUp(e) {
    if (e.code === 'ArrowLeft' || e.code === 'ArrowRight') {
      this._stopDAS();
    }
  }

  _startDAS(action) {
    this._stopDAS();
    this.dasTimer = setTimeout(() => {
      this.dasRepeat = setInterval(action, this.dasInterval);
    }, this.dasDelay);
  }

  _stopDAS() {
    clearTimeout(this.dasTimer);
    clearInterval(this.dasRepeat);
    this.dasTimer = null;
    this.dasRepeat = null;
  }

  destroy() {
    document.removeEventListener('keydown', this._onKeyDown);
    document.removeEventListener('keyup', this._onKeyUp);
    this._stopDAS();
  }
}
