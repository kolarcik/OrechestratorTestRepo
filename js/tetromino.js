const TETROMINOES = {
  I: {
    shape: [[0,0,0,0],[1,1,1,1],[0,0,0,0],[0,0,0,0]],
    color: '#00f0f0'
  },
  O: {
    shape: [[1,1],[1,1]],
    color: '#f0f000'
  },
  T: {
    shape: [[0,1,0],[1,1,1],[0,0,0]],
    color: '#a000f0'
  },
  S: {
    shape: [[0,1,1],[1,1,0],[0,0,0]],
    color: '#00f000'
  },
  Z: {
    shape: [[1,1,0],[0,1,1],[0,0,0]],
    color: '#f00000'
  },
  J: {
    shape: [[1,0,0],[1,1,1],[0,0,0]],
    color: '#0000f0'
  },
  L: {
    shape: [[0,0,1],[1,1,1],[0,0,0]],
    color: '#f0a000'
  }
};

function rotateCW(shape) {
  const N = shape.length;
  const result = Array.from({ length: N }, () => Array(N).fill(0));
  for (let r = 0; r < N; r++) {
    for (let c = 0; c < shape[r].length; c++) {
      result[c][N - 1 - r] = shape[r][c];
    }
  }
  return result;
}

function rotateCCW(shape) {
  const N = shape.length;
  const result = Array.from({ length: N }, () => Array(N).fill(0));
  for (let r = 0; r < N; r++) {
    for (let c = 0; c < shape[r].length; c++) {
      result[N - 1 - c][r] = shape[r][c];
    }
  }
  return result;
}

function randomTetromino() {
  const keys = Object.keys(TETROMINOES);
  const key = keys[Math.floor(Math.random() * keys.length)];
  const t = TETROMINOES[key];
  return {
    shape: t.shape.map(row => [...row]),
    color: t.color
  };
}
