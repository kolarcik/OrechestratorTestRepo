# Úkol: Backend – Tetris High Score API

## Přehled
Implementuj jednoduchý REST API server pro ukládání a načítání high scores pro Tetris hru.

## Technologie
- Node.js
- Express.js
- CORS middleware
- JSON file storage (scores.json) – bez databáze

## Struktura souborů – vytvoř přesně toto:
```
server.js
scores.json      # inicializuj jako prázdné pole: []
package.json     # vytvořit pomocí npm init
```

## Detailní specifikace

### package.json
Spusť `npm init -y` a pak `npm install express cors`.
Přidej do scripts:
```json
"start": "node server.js"
```

### scores.json
Inicializuj jako prázdné JSON pole:
```json
[]
```

### server.js – kompletní implementace

```javascript
const express = require('express');
const cors = require('cors');
const fs = require('fs');
const path = require('path');

const app = express();
const PORT = process.env.PORT || 3000;
const SCORES_FILE = path.join(__dirname, 'scores.json');
const MAX_RECORDS = 100;

// Middleware
app.use(cors());
app.use(express.json());

// Helper: načtení scores ze souboru
function loadScores() {
  try {
    const data = fs.readFileSync(SCORES_FILE, 'utf8');
    return JSON.parse(data);
  } catch {
    return [];
  }
}

// Helper: uložení scores do souboru
function saveScores(scores) {
  fs.writeFileSync(SCORES_FILE, JSON.stringify(scores, null, 2));
}

// GET /api/scores – vrátí top 10 sestupně podle score
app.get('/api/scores', (req, res) => {
  const scores = loadScores();
  const top10 = scores
    .sort((a, b) => b.score - a.score)
    .slice(0, 10);
  res.json(top10);
});

// POST /api/scores – uloží nové skóre
app.post('/api/scores', (req, res) => {
  const { name, score, level, lines } = req.body;

  // Validace
  if (!name || typeof name !== 'string' || name.trim().length === 0) {
    return res.status(400).json({ error: 'name is required' });
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

  // Zachovat max MAX_RECORDS záznamů (zahazuj nejhorší)
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
```

## Spuštění
```bash
npm install
node server.js
# nebo
npm start
```

Server poběží na http://localhost:3000

## Testování endpointů
```bash
# GET scores
curl http://localhost:3000/api/scores

# POST score
curl -X POST http://localhost:3000/api/scores \
  -H "Content-Type: application/json" \
  -d '{"name":"TEST","score":1500,"level":3,"lines":15}'
```

## Důležité
- Server musí povolovat CORS pro všechny origins (hra může běžet z file://)
- scores.json se vytváří v adresáři projektu, ne globálně
- Chyby při čtení souboru = prázdný seznam (nepadej)
