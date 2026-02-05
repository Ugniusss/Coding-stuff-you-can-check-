```html
<!doctype html>
<meta charset="utf-8">
<meta name=viewport content="width=device-width,initial-scale=1">
<title>AMG8833</title>
<style>
    body{font-family:system-ui;margin:16px}
    canvas{image-rendering:pixelated;border-radius:12px;box-shadow:0 6px 20px rgba(0,0,0,.1);max-width:90vw;height:auto}
    .row{display:flex;gap:12px;align-items:center;flex-wrap:wrap}
    .badge{padding:.2em .6em;border-radius:999px;background:#eee}
    .btn{padding:.3em .8em;border:1px solid #bbb;border-radius:10px;cursor:pointer}
    .card{max-width:900px}
    .small{opacity:.7}
</style>

<div class=card>
    <h2>Šiluminis vaizdas (8×8)</h2>
    <canvas id=c></canvas>
    <div class=row>
        <span class=badge id=stat>—</span>
        <span>Min: <b id=mn>—</b></span>
        <span>Max: <b id=mx>—</b></span>
        <span>Avg: <b id=avg>—</b></span>
        <button class=btn onclick="fetch('/toggle')">ON/OFF matricą</button>
    </div>
</div>

<script>
    const W=8,H=8, SCALE=24;
    const FLIP_X = true;
    const FLIP_Y = false;

    const cvs=document.getElementById('c');
    cvs.width=W*SCALE;
    cvs.height=H*SCALE;
    const ctx=cvs.getContext('2d');

    function color(t){const r=255*t,g=255*t*t*t,b=255*(1-t);return [r|0,g|0,b|0];}

    function draw(vals){
        let mn=Math.min(...vals), mx=Math.max(...vals);
        let avg=vals.reduce((a,b)=>a+b,0)/vals.length;
        document.getElementById('mn').textContent=mn.toFixed(1)+' °C';
        document.getElementById('mx').textContent=mx.toFixed(1)+' °C';
        document.getElementById('avg').textContent=avg.toFixed(1)+' °C';
        document.getElementById('stat').textContent='Atnaujinta '+new Date().toLocaleTimeString();

        const lo=mn, hi=mx==mn?mn+1:mx;
        const img=ctx.createImageData(W*SCALE,H*SCALE);

        for(let y=0;y<H;y++)for(let x=0;x<W;x++){
            const v = vals[y*W + x];
            const t = (v - lo) / (hi - lo);
            const [r,g,b] = color(t);
            const px = FLIP_X ? (W-1-x) : x;
            const py = FLIP_Y ? (H-1-y) : y;

            for(let dy=0;dy<SCALE;dy++)for(let dx=0;dx<SCALE;dx++){
                const X=px*SCALE+dx,Y=py*SCALE+dy,i=(Y*cvs.width+X)*4;
                img.data[i]=r;img.data[i+1]=g;img.data[i+2]=b;img.data[i+3]=255;
            }
        }
        ctx.putImageData(img,0,0);
    }

    const es=new EventSource('/events');
    es.onmessage=e=>{const d=JSON.parse(e.data); if(d.temps) draw(d.temps);};
</script>

</pre>
