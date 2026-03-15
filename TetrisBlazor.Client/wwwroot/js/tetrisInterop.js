window.tetrisInterop = {
    dotNetRef: null,
    _boundHandler: null,

    init: function (dotNetReference) {
        this.dotNetRef = dotNetReference;
        this._boundHandler = this._handleKey.bind(this);
        document.addEventListener('keydown', this._boundHandler);
    },

    dispose: function () {
        if (this._boundHandler) {
            document.removeEventListener('keydown', this._boundHandler);
            this._boundHandler = null;
        }
        this.dotNetRef = null;
    },

    _handleKey: function (e) {
        const gameKeys = ['ArrowLeft', 'ArrowRight', 'ArrowDown', 'ArrowUp', 'Space', 'KeyX', 'KeyZ', 'KeyP'];
        if (gameKeys.includes(e.code)) e.preventDefault();
        if (this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('HandleKey', e.code);
        }
    }
};
