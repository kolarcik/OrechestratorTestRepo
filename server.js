const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;
const SCORES_FILE = path.join(__dirname, 'scores.json');
const MAX_RECORDS = 100;

app.use(cors());
app.use(express.json());

function loadScores() {
  try {
    const data = fs.readFileSync(SCORES_FILE, 'utf8');
    return JSON.parse(data);
  } catch {
    return [];
  }
}

function saveScores(scores) {
  fs.writeFileSync(SCORES_FILE, JSON.stringify(scores, null, 2));
}

app.get('/api/scores', (req, res) => {
  const scores = loadScores();
  const top10 = scores
    .sort((a, b) => b.score - a.score)
    .slice(0, 10);
  res.json(top10);
});

app.post('/api/scores', (req, res) => {
  const { name, score, level, lines } = req.body;

  if (!name || typeof name !== 'string' || name.trim().length === 0) {
    return res.status(400).json({ error: 'name is required' });
  }
  if (name.trim().length > 20) {
    return res.status(400).json({ error: 'name must be max 20 chars' });
  }
  if (typeof score !== 'number' || score < 0) {
    return res.status(400).json({ error: 'score must be a non-negative number' });
  }
  if (typeof level !== 'number' || level < 1) {
    return res.status(400).json({ error: 'level must be >= 1' });
  }
  if (typeof lines !== 'number' || lines < 0) {
    return res.status(400).json({ error: 'lines must be a non-negative number' });
  }

  const newRecord = {
    name: name.trim().slice(0, 20),
    score,
    level,
    lines,
    date: new Date().toISOString().split('T')[0]
  };

  let scores = loadScores();
  scores.push(newRecord);

  if (scores.length > MAX_RECORDS) {
    scores = scores
      .sort((a, b) => b.score - a.score)
      .slice(0, MAX_RECORDS);
  }

  saveScores(scores);
  res.status(201).json(newRecord);
});

app.listen(PORT, () => {
  console.log(`Tetris Score API running on http://localhost:${PORT}`);
});
