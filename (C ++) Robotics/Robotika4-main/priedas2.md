<pre>
import time
import requests
import pyautogui

ESP_IP = "192.168.4.1"              # SilumosWeb IP
URL_POS = f"http://{ESP_IP}/pos"

CENTER_X = 3.5                      # 8x8 centro koordinatės
CENTER_Y = 3.5

SCALE     = 20                       # kiek px per 1 „langelį“ (mažiau = švelniau)
MAX_STEP  = 20                      # max px per iteraciją
INTERVAL  = 0.008                    # s tarp iteracijų (~33 Hz)

# Smoothing parametras (0..1) – kuo didesnis, tuo greičiau vejasi
ALPHA = 0.5

pyautogui.FAILSAFE = False

smooth_dx = 0.0
smooth_dy = 0.0

print("Prisijunk prie Wi-Fi 'SilumosWeb' (IP ~ 192.168.4.1).")
time.sleep(2)
print("Start termo-pelytė (uždaryk langą, jei nori sustabdyti).")

while True:
    try:
        r = requests.get(URL_POS, timeout=0.2)
        d = r.json()
        hx = d.get("hx", CENTER_X)
        hy = d.get("hy", CENTER_Y)

        # ---- 4x4 CENTRINĖ DEADZONE ----
        # jei karščiausias taškas 2..5 x ir 2..5 y – pelė stovi vietoje
        if 2 <= hx <= 5 and 2 <= hy <= 5:
            target_dx = 0.0
            target_dy = 0.0
        else:
            # poslinkis nuo centro
            target_dx = hx - CENTER_X
            target_dy = hy - CENTER_Y


        # ---- SMOOTH FILTRAS (exponential smoothing) ----
        smooth_dx = smooth_dx * (1 - ALPHA) + target_dx * ALPHA
        smooth_dy = smooth_dy * (1 - ALPHA) + target_dy * ALPHA

        # paverčiam į pikselius
        move_x = int(smooth_dx * SCALE)
        move_y = int(smooth_dy * SCALE)

        # apribojam max žingsnį
        if move_x >  MAX_STEP: move_x =  MAX_STEP
        if move_x < -MAX_STEP: move_x = -MAX_STEP
        if move_y >  MAX_STEP: move_y =  MAX_STEP
        if move_y < -MAX_STEP: move_y = -MAX_STEP

        if abs(move_x) >= 1 or abs(move_y) >= 1:
            pyautogui.moveRel(move_x, move_y, duration=INTERVAL)
        else:
            time.sleep(INTERVAL)

    except Exception as e:
        print("Klaida:", e)
        time.sleep(1)

</pre>
