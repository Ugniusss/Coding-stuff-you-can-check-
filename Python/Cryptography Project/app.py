#
# Step-by-step decoding
#


from flask import Flask, request, jsonify, send_from_directory
from functions import EncodeDecode, stringToBinList, binListToString
import functions as func
from PIL import Image
import io, base64

app = Flask(__name__, static_folder=".", static_url_path="")

# ===================== UI =====================

@app.route("/")
def index():
    return send_from_directory(".", "net.html")

# ===================== PAGALBINĖS =====================

def bits_to_G(bits, n, k):
    G = []
    idx = 0
    for _ in range(k):
        G.append([int(b) for b in bits[idx:idx+n]])
        idx += n
    return G

def get_G(G_bits, n, k):
    if G_bits:
        return bits_to_G(G_bits, n, k)
    return func.RandonGeneruojantiM(n, k)

# ===================== GENERUOTI G =====================

@app.post("/api/generate_G")
def generate_G():
    d = request.json
    n = int(d["n"])
    k = int(d["k"])
    G = func.RandonGeneruojantiM(n, k)
    return jsonify({"G": G})

# ===================== 1. VEKTORIUS =====================

@app.post("/api/vector")
def vector():
    d = request.json
    n = int(d["n"])
    k = int(d["k"])
    pe = float(d["pe"])
    msg = []
    for x in d["message"]:
        msg.append(int(x))

    G = get_G(d.get("G_bits"), n, k)

    encoded = func.MkartM(msg, G)
    received = func.Kanalas(encoded, pe)

    H = func.Hmatrica(G)
    leaders = func.CosetLeader(H)
    decoded = func.StepByStep(leaders, H, received)

    return jsonify({
        "encoded": encoded[0],
        "received": received[0],
        "decoded": decoded[0][:k]
    })
@app.post("/api/decode")
def decode_only():
    d = request.json

    n = int(d["n"])
    k = int(d["k"])

    received = [int(x) for x in d["received"]]

    G = get_G(d.get("G_bits"), n, k)

    H = func.Hmatrica(G)
    coset_leaders = func.CosetLeader(H)

    decoded = func.StepByStep(coset_leaders, H, [received])

    return jsonify({
        "decoded": decoded[0][:k]
    })

# ===================== 2. TEKSTAS =====================

@app.post("/api/text")
def text():
    d = request.json
    n = int(d["n"])
    k = int(d["k"])
    pe = float(d["pe"])
    text = d["text"]

    G = get_G(d.get("G_bits"), n, k)

    bits = stringToBinList(text)
    no_code, with_code = EncodeDecode(bits, G, pe)

    return jsonify({
        "no_code": binListToString(no_code),
        "with_code": binListToString(with_code)
    })

# ===================== 3. BMP =====================

@app.post("/api/bmp")
def bmp():
    f = request.files["file"]
    n = int(request.form["n"])
    k = int(request.form["k"])
    pe = float(request.form["pe"])

    G = get_G(request.form.get("G_bits"), n, k)

    img = Image.open(f).convert("RGB")
    w, h = img.size
    pixels = list(img.getdata())

    bits = []
    for r, g, b in pixels:
        bits += list(map(int, f"{r:08b}{g:08b}{b:08b}"))

    no_code, with_code = EncodeDecode(bits, G, pe)

    def bitsToImg(bitlist):
        px = []
        i = 0
        for _ in range(w * h):
            r = int("".join(map(str, bitlist[i:i+8])), 2); i+=8
            g = int("".join(map(str, bitlist[i:i+8])), 2); i+=8
            b = int("".join(map(str, bitlist[i:i+8])), 2); i+=8
            px.append((r,g,b))
        im = Image.new("RGB", (w,h))
        im.putdata(px)
        buf = io.BytesIO()
        im.save(buf, format="BMP")
        return "data:image/bmp;base64," + base64.b64encode(buf.getvalue()).decode()

    return jsonify({
        "original": bitsToImg(bits),
        "no_code": bitsToImg(no_code),
        "with_code": bitsToImg(with_code)
    })

# ===================== START =====================

if __name__ == "__main__":
    app.run(debug=True)
