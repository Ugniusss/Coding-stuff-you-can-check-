# ESP32-S3 — Šiluminio vaizdo sistema

Šis projektas sukuria Wi-Fi prieigos tašką ir realiu laiku vizualizuoja šilumos pasiskirstymą 8×8 tinklelyje, naudodamas **Adafruit AMG8833** infraraudonųjų spindulių jutiklį. Duomenys rodomi per **MAX7219 LED matricą** ir naršyklėje per integruotą **web serverį**.  
Sistema išsaugo nustatymus „flash“ atmintyje (Preferences) ir leidžia junginėti LED matricą mygtuku ar per tinklalapį.

---

## 1. Sistemos aprašymas

- **ESP32-S3** renka 8×8 šilumos taškus (°C) iš AMG8833.
- **LED matricoje** parodomas karščiausias taškas.
- **Web puslapis** rodo visą šiluminį vaizdą spalvų forma.
- **Mygtukas** įjungia/išjungia LED matricą.
- Duomenys atnaujinami ~10 kartų per sekundę.

---

## 2. Naudoti komponentai

| Komponentas | Aprašymas | Sujungimas su ESP32-S3 |
|--------------|------------|-------------------------|
| **ESP32-S3 N16R8** | Pagrindinis valdiklis, Wi-Fi + flash | – |
| **AMG8833** | 8×8 IR šiluminis jutiklis (I²C) | SDA → GPIO 8<br>SCL → GPIO 9<br>3.3V / GND |
| **MAX7219 8×8 matrica** | LED vizualizacija | DIN → GPIO 11<br>CLK → GPIO 10<br>CS → GPIO 12<br>VCC / GND |
| **Mygtukas** | Matricos įjungimas/išjungimas | Vienas galas → GPIO 15<br>Kitas → GND |


---

## 3. Programinės įrangos architektūra

Sistema asinchroninė ir įvykių valdoma:
- **Timer interrupt** kas 100 ms nustato `frameFlag = true`.
- **Main loop** nuskaito AMG8833, randa karščiausią tašką, atnaujina matricą ir siunčia JSON į naršyklę.
- **Button interrupt** keičia `matrixEnabled` būseną ir saugo į Preferences.
- **Web serveris** (AsyncWebServer) pateikia `index.html` ir realaus laiko duomenis per **/events** (Server-Sent Events).

---

## 4. Naudojimas

1. Įkeliamas `index.html` į **/data/** ir paleidžiamas **ESP32 LittleFS Data Upload**.  
2. Įkeliamas pagrindini `.ino` projektas.  
3. Prisigiama prie Wi-Fi tinklo **SilumosWeb**.  
4. Atidaroma naršyklėje `192.168.4.1` ir matomas šilumos žemėlapis.  
5. Mygtuku arba `/toggle` adresu galima išjungti/įjungti LED matricą.

---

## 5. Failų sistema

- `index.html` – tinklalapis, kuriame piešiamas 8×8 šiluminis vaizdas.
- `Preferences` (flash) – saugoma:
  - `mx_on` – matricos būsena (ON/OFF)
  - `recMaxC`, `recX`, `recY` – didžiausia temperatūra ir koordinatės

---

