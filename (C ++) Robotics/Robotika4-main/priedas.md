<pre>
  <!DOCTYPE html>
<html lang="lt">
<head>
  <meta charset="UTF-8">
  <title>Šilumos Web + Minigame</title>
  <style>
    body {
      font-family: sans-serif;
      background: #111;
      color: #eee;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 16px;
      margin: 0;
      padding: 16px;
    }
    h1 { margin: 0 0 4px; }

    .wrap {
      display: flex;
      flex-wrap: wrap;
      gap: 20px;
    }
    canvas {
      background: #000;
      image-rendering: pixelated;
    }
    .card {
      padding: 10px;
      border-radius: 8px;
      background: #222;
      border: 1px solid #444;
      min-width: 260px;
    }
    button {
      padding: 6px 12px;
      border-radius: 6px;
      border: 1px solid #666;
      background: #333;
      color: #eee;
      cursor: pointer;
    }
    button:hover {
      background: #444;
    }
    #status {
      font-size: 0.9em;
      opacity: 0.9;
      margin-left: 8px;
      font-weight: bold;
    }
  </style>
</head>
<body>
  <h1>Šilumos Web</h1>
  <div class="card">
    <button id="toggleBtn">Perjungti matricą</button>
    <span id="status">Būsena: ?</span>
  </div>

  <div class="wrap">
    <!-- Šilumos žemėlapis -->
    <div class="card">
      <h3>8×8 šilumos žemėlapis</h3>
      <canvas id="heat" width="240" height="240"></canvas>
      <p style="margin-top:8px;font-size:0.9em;">
        Vidutinė: <span id="avgTemp">-</span> °C<br>
        Karščiausia: <span id="maxTemp">-</span> °C
      </p>
    </div>

    <!-- Minigame -->
    <div class="card">
      <h3>Minigame: paliesk raudoną kvadratą</h3>
      <p style="margin-top:0;font-size:0.9em;">
        Balta = tavo pelytė (karščiausias taškas), raudonas 3×3 kvadratas = taikinys.<br>
        Uždėk baltą ant raudono kvadrato, kad surinktum tašką.
      </p>
      <canvas id="pos" width="240" height="240"></canvas>
      <p>Surinkta: <strong id="score">0</strong></p>
    </div>
  </div>

  <script>
    const cellSize = 30; // 8 * 30 = 240 px

    const heatCanvas = document.getElementById('heat');
    const heatCtx = heatCanvas.getContext('2d');

    const posCanvas = document.getElementById('pos');
    const posCtx = posCanvas.getContext('2d');

    const statusSpan = document.getElementById('status');
    const toggleBtn = document.getElementById('toggleBtn');
    const scoreSpan = document.getElementById('score');
    const avgTempSpan = document.getElementById('avgTemp');
    const maxTempSpan = document.getElementById('maxTemp');

    // paskutinė /pos pozicija (kad prireikus galėtume dėti overlay ant heatmap)
    let lastHx = 0;
    let lastHy = 0;
    let lastTemps = null;

    // ---- Šilumos žemėlapis iš /events ----
    function drawHeat() {
      if (!Array.isArray(lastTemps) || lastTemps.length !== 64) return;

      const temps = lastTemps;

      let min = Math.min(...temps);
      let max = Math.max(...temps);
      if (max === min) max = min + 1;

      let sum = 0;
      for (let i = 0; i < temps.length; i++) sum += temps[i];
      const avg = sum / temps.length;
      const hi = max;

      avgTempSpan.textContent = avg.toFixed(1);
      maxTempSpan.textContent = hi.toFixed(1);

      for (let i = 0; i < 64; i++) {
        const t = temps[i];
        const x = i % 8;
        const y = Math.floor(i / 8);
        const n = (t - min) / (max - min);   // 0..1
        const r = Math.floor(255 * n);
        const g = 0;
        const b = Math.floor(255 * (1 - n));

        heatCtx.fillStyle = `rgb(${r},${g},${b})`;
        heatCtx.fillRect(x * cellSize, y * cellSize, cellSize, cellSize);
      }

      heatCtx.fillStyle = 'white';
      heatCtx.beginPath();
      heatCtx.arc(
        (lastHx + 0.5) * cellSize,
        (lastHy + 0.5) * cellSize,
        cellSize * 0.25,
        0,
        Math.PI * 2
      );
      heatCtx.fill();
    }

    if (!!window.EventSource) {
      const es = new EventSource('/events');
      es.onmessage = (e) => {
        try {
          const data = JSON.parse(e.data);
          if (data.temps) {
            lastTemps = data.temps;
            drawHeat();
          }
        } catch (err) {
          console.error('SSE error:', err);
        }
      };
      es.onerror = () => {
        console.warn('SSE atjungta');
      };
    }

    // ---- Minigame kintamieji ----
    let playerHx = 0;
    let playerHy = 0;
    let targetHx = 4;
    let targetHy = 4;
    let score = 0;

    function randomTarget() {
      const nx = 1 + Math.floor(Math.random() * 6);
      const ny = 1 + Math.floor(Math.random() * 6);
      targetHx = nx;
      targetHy = ny;
    }

    function drawGame() {
      posCtx.clearRect(0, 0, posCanvas.width, posCanvas.height);

      // Tinklelis
      posCtx.strokeStyle = '#333';
      for (let i = 0; i <= 8; i++) {
        posCtx.beginPath();
        posCtx.moveTo(i * cellSize, 0);
        posCtx.lineTo(i * cellSize, 8 * cellSize);
        posCtx.stroke();

        posCtx.beginPath();
        posCtx.moveTo(0, i * cellSize);
        posCtx.lineTo(8 * cellSize, i * cellSize);
        posCtx.stroke();
      }

      // Taikinys – raudonas 3x3 kvadratas aplink targetHx, targetHy
      const sizeCells = 3;
      const half = 1; // (3-1)/2
      const startX = (targetHx - half) * cellSize;
      const startY = (targetHy - half) * cellSize;

      posCtx.fillStyle = 'red';
      posCtx.fillRect(
        startX,
        startY,
        sizeCells * cellSize,
        sizeCells * cellSize
      );

      // Žaidėjo pozicija – balta „pelytė“
      posCtx.fillStyle = 'white';
      posCtx.beginPath();
      posCtx.arc(
        (playerHx + 0.5) * cellSize,
        (playerHy + 0.5) * cellSize,
        cellSize * 0.3,
        0,
        Math.PI * 2
      );
      posCtx.fill();
    }

    function checkHit() {
      // 3x3 – atstumas <= 1 abiem ašim
      if (Math.abs(playerHx - targetHx) <= 1 && Math.abs(playerHy - targetHy) <= 1) {
        score++;
        scoreSpan.textContent = score.toString();
        randomTarget();
      }
    }

    async function pollPos() {
      try {
        const r = await fetch('/pos');
        if (!r.ok) throw new Error('HTTP ' + r.status);
        const j = await r.json();
        if (typeof j.hx === 'number' && typeof j.hy === 'number') {
          // VIENINTELIS interpretavimas: tos pačios hx,hy visur
          playerHx = j.hx;
          playerHy = j.hy;

          // saugom ir heatmap overlay'ui
          lastHx = j.hx;
          lastHy = j.hy;

          checkHit();
          drawGame();
          // ir perpiešiam heatmap, jei turim temp duomenis
          if (lastTemps !== null) {
            drawHeat();
          }
        }
      } catch (e) {
        console.warn('Nepavyko gauti /pos:', e.message);
      }
    }

    setInterval(pollPos, 100); // ~10 Hz

    // ---- Web mygtukas matricai ----
    function updateStatusFromReply(text) {
      if (text.indexOf('matrix:on') !== -1) {
        statusSpan.textContent = 'Būsena: ON';
      } else if (text.indexOf('matrix:off') !== -1) {
        statusSpan.textContent = 'Būsena: OFF';
      } else {
        statusSpan.textContent = 'Būsena: ?';
      }
    }

    toggleBtn.addEventListener('click', async () => {
      try {
        const r = await fetch('/toggle');
        const text = await r.text();
        updateStatusFromReply(text);
      } catch (e) {
        statusSpan.textContent = 'Būsena: klaida';
      }
    });

    randomTarget();
    drawGame();
  </script>
</body>
</html>

</pre>
