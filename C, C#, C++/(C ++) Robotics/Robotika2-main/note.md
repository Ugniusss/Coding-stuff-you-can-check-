# Ugnius Padolskis Informatika 4 kursas 1 grupė
# ESP32-C3 Super Mini — Wi-Fi RSSI Tracker

Šis projektas skirtas matuoti ir vizualizuoti Wi-Fi signalo stiprumą (RSSI) naudojant ESP32-C3 Super Mini mikrokontrolerį.  
Sistema periodiškai fiksuoja signalo stiprumą, apdoroja duomenis asinchroniškai, rodo juos LCD ekrane ir LED indikatoriumi.  
Vartotojas gali keisti veikimo režimus mygtuko paspaudimu, o nustatymai išsaugomi vidinėje atmintyje (EEPROM/Preferences).

---

## 1. Sistemos aprašymas

Įrenginys skirtas parodyti esamo Wi-Fi signalo stiprumą trijuose režimuose:
- **Live:** realaus laiko RSSI reikšmė (dBm).
- **Average:** slenkantis paskutinių 10 matavimų vidurkis.
- **Bars:** vizualizuotas signalas stulpeliais (0–5 padalos).

Be LCD rodmens, LED indikatorius pateikia greitą signalo būsenos informaciją:
- > -40 dBm – LED šviečia pastoviai (stiprus signalas),
- -40 … -60 dBm – LED greitai mirksi (vidutinis signalas),
- < -60 dBm – LED lėtai mirksi (silpnas signalas).

---

## 2. Naudoti komponentai

| Komponentas | Aprašymas |
|--------------|-----------|
| ESP32-C3 Super Mini | Mikrovaldiklis su integruotu Wi-Fi moduliu |
| LCD 16x2 (I²C, 0x27) | Ekranas RSSI reikšmėms ir režimams |
| Mygtukas | Režimų keitimui (external interrupt) |
| LED + 220 Ω rezistorius | Signalo stiprumo indikacija |
| Breadboard + laidai | Prototipavimui ir testavimui |

**Komponentų nuotrauka:**  
![Komponentai](https://github.com/Ugniusss/Robotika2/blob/main/components.png)

**Sujungimo schema:**  
![Schema](https://github.com/Ugniusss/Robotika2/blob/main/circuit.png)

---

## 3. Programinės įrangos architektūra

Sistema sukurta remiantis **asinchroniniu, įvykių valdomu (event-driven)** principu.  
Vengiama „busy-wait“ konstrukcijų ir `delay()` funkcijų, vietoj to naudojami **pertraukimai (interrupts)** ir **hardware timeris**.

Pagrindiniai valdymo elementai:
- **Timer interrupt** – nustato `sampleFlag = true` kas 1 sekundę.
- **Main loop** – tikrina flag'ą, nuskaito RSSI, atnaujina ekraną ir LED.
- **Button interrupt** – keičia veikimo režimą ir išsaugo jį į atmintį.

---

## 5. Timer, laiko biudžetas ir EEPROM

**Timer konfigūracija:**  
Naudojamas ESP32-C3 hardware timer, kuris kas 1 sekundę sukelia pertrauktį (`onTimer()`), nustatančią `sampleFlag`.  
Tokiu būdu matavimai vyksta periodiškai ir neblokuoja pagrindinio ciklo (`loop()`).

**Laiko biudžetas:**  
RSSI matavimas ir LCD atnaujinimas trunka apie 7 ms kas sekundę.  
Mygtuko ir LED valdymas vyksta mikrosekundžių tikslumu, todėl sistema veikia beveik be vėlinimų (<1% CPU apkrovos).

**EEPROM (Preferences):**  
ESP32 neturi atskiro EEPROM, todėl naudojama `Preferences` biblioteka (flash atmintis).  
Išsaugomi duomenys:
- `mode` – paskutinis veikimo režimas (Live / Avg / Bars)  
- `cal_rssi` – paskutinis vidutinis RSSI  
Reikšmės išlieka net išjungus maitinimą.


