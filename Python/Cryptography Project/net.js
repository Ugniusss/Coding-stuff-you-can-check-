// ================= PAGALBINĖS =================
function params() {
    return { n:+n.value, k:+k.value, pe:+pe.value };
}

function collectG() {
    return customG.value.trim().split("\n").join("");
}

function collectAutoG() {
    return autoG.value.trim().split("\n").join("");
}

function formatG(G) {
    return G.map(r => r.join("")).join("\n");
}

// ================= G GENERAVIMAS =================
async function genG() {
    const {n,k} = params();

    const r = await fetch("/api/generate_G", {
        method:"POST",
        headers:{"Content-Type":"application/json"},
        body:JSON.stringify({n,k})
    });

    const d = await r.json();
    autoG.value = formatG(d.G);
    customG.value = formatG(d.G);
}

// ================= 1. VEKTORIUS =================


async function Encode_C() {
    const { n, k, pe } = params();
    const message = vecInput.value;

    const d = await EncodeVRequest({
        n, k, pe,
        message,
        G_bits: collectG()
    });

    vecEncoded.value = d.encoded.join("");
    vecReceived.value = d.received.join("");

    let errs = [];
    for (let i = 0; i < d.encoded.length; i++)
        if (d.encoded[i] != d.received[i]) errs.push(i + 1);

    vecErrors.textContent = errs.length ? errs.join(", ") : "nėra";
}


async function Encode_A() {
    const { n, k, pe } = params();
    const message = vecInput.value;

    const d = await EncodeVRequest({
        n, k, pe,
        message,
        G_bits: collectAutoG()
    });

    vecEncoded.value = d.encoded.join("");
    vecReceived.value = d.received.join("");

    let errs = [];
    for (let i = 0; i < d.encoded.length; i++)
        if (d.encoded[i] != d.received[i]) errs.push(i + 1);

    vecErrors.textContent = errs.length ? errs.join(", ") : "nėra";
}


async function EncodeVRequest(payload) {
    const r = await fetch("/api/vector", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });
    return await r.json();
}


async function decodeVector(useCustomG) {
    const received = vecReceived.value.trim();

    if (!received) {
        alert("Nėra gauto vektoriaus");
        return;
    }

    // paprasta validacija
    if (received.length !== +n.value) {
        alert("Gauto vektoriaus ilgis turi būti n");
        return;
    }

    const r = await fetch("/api/decode", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            n: +n.value,
            k: +k.value,
            received: received,
            G_bits: useCustomG ? collectG() : collectAutoG()
        })
    });

    const d = await r.json();
    vecDecoded.value = d.decoded.join("");
}


// ================= 2. TEKSTAS =================
async function runText(useCustomG) {
    const r = await fetch("/api/text", {
        method:"POST",
        headers:{"Content-Type":"application/json"},
        body:JSON.stringify({
            ...params(),
            text: textInput.value,
            G_bits: useCustomG ? collectG() : collectAutoG()
        })
    });

    const d = await r.json();
    textNoCode.value = d.no_code;
    textWithCode.value = d.with_code;
}

// ================= 3. BMP =================
async function runBMP(useCustomG) {
    const file = bmp.files[0];
    if (!file) return;

    bmpO.src = URL.createObjectURL(file);

    const fd = new FormData();
    fd.append("file", file);
    fd.append("n", n.value);
    fd.append("k", k.value);
    fd.append("pe", pe.value);

    if (useCustomG) fd.append("G_bits", collectG());

    const r = await fetch("/api/bmp", { method:"POST", body: fd });
    const d = await r.json();

    bmpN.src = d.no_code;
    bmpW.src = d.with_code;
}


