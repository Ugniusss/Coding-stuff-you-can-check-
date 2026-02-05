# ESP32-S3 — Šiluminio vaizdo sistema + mini žaidimas + fake pelytė

Šis projektas sukuria Wi-Fi prieigos tašką ir realiu laiku vizualizuoja šilumos pasiskirstymą 8×8 tinklelyje, naudodamas **Adafruit AMG8833** infraraudonųjų spindulių jutiklį.  

Duomenys naudojami trim būdais:

- rodomas **karščiausias taškas** per **MAX7219 LED matricą**;
- naršyklėje rodomas **spalvotas šilumos žemėlapis** ir **mini žaidimas**;
- per atskirą **Python skriptą** kompiuteryje karščiausias taškas valdo **pelės judėjimą** ;

Sistema išsaugo nustatymus „flash“ atmintyje (Preferences) ir leidžia junginėti LED matricą **mygtuku** arba per web API.

---

## 1. Sistemos aprašymas

- **ESP32-S3** renka 8×8 šilumos taškus (°C) iš AMG8833.
- Iš duomenų apskaičiuojamas **karščiausias pikselis** (hx, hy).
- **LED matricoje** parodomas karščiausias taškas (3×3 kvadratu).
- ESP sukuria Wi-Fi AP **„SilumosWeb“** su „captive portal“ (telefonai/laptopai automatiškai atsidaro puslapį).
- **Web puslapyje** (`index.html`):
  - rodomas visas 8×8 šiluminis vaizdas spalvų forma;
  - rodomas **mini žaidimas**: baltas taškas (tavo šilumos taškas) turi „paliesti“ atsitiktinį raudoną taikinį, skaičiuojamas **score**.
- Per papildomą endpointą **`/pos`** grąžinamos karščiausio taško koordinatės `{hx, hy}`.
  - Šias koordinatės naudoja **Python skriptas kompiuteryje**, kad judintų pelės žymeklį pagal šilumos tašką.
- **Mygtukas** įjungia/išjungia LED matricą.
- Duomenys atnaujinami ~10 kartų per sekundę (timeris kas 100 ms).

---

## 2. Naudoti komponentai

| Komponentas            | Aprašymas                                | Sujungimas su ESP32-S3                     |
|------------------------|-------------------------------------------|--------------------------------------------|
| **ESP32-S3 N16R8**     | Pagrindinis valdiklis, Wi-Fi + flash      | –                                          |
| **AMG8833**            | 8×8 IR šiluminis jutiklis (I²C)           | SDA → GPIO 8<br>SCL → GPIO 9<br>3.3V / GND |
| **MAX7219 8×8 matrica**| LED vizualizacija                         | DIN → GPIO 11<br>CLK → GPIO 10<br>CS → GPIO 12<br>VCC / GND |
| **Mygtukas**           | Matricos įjungimas/išjungimas             | Vienas galas → GPIO 15<br>Kitas → GND      |

---

## 3. Programinės įrangos architektūra (ESP32)

Sistema asinchroninė ir įvykių valdoma:

- **Timer interrupt** kas 100 ms:
  - nustato `frameFlag = true`;
- **Main loop**:
  - laukia `frameFlag`, tada nuskaito 64 AMG8833 pikselius;
  - suranda karščiausią tašką (`hiIdx`, `hiVal`) ir apskaičiuoja `hx`, `hy`;
  - išsaugo `lastHx` ir `lastHy` globaliuose kintamuosiuose (naudojami `/pos`);
  - atnaujina LED matricą (`drawHotspotOnMatrix(hx, hy)`);
  - jei temperatūra viršija rekordą – atnaujina `recMaxC`, `recX`, `recY` ir išsaugo juos per `Preferences`;
  - suformuoja JSON `{"temps":[...64 reikšmės...]}` ir išsiunčia per **Server-Sent Events** (`events.send(...)`) į web puslapį.

- **Button interrupt**:
  - detektuoja mygtuko paspaudimą;
  - per „debouncing“ logiką main loope perjungia `matrixEnabled` ir, jei išjungta, nuvalo LED matricą;
  - būseną išsaugo „flash“ atmintyje (`Preferences`).

- **Web serveris** (`AsyncWebServer`):
  - `"/"` – pateikia `index.html` iš **LittleFS**;
  - keli sistemos URL (Android captive portal, Microsoft NCSI ir pan.) peradresuojami į `/`;
  - `"/toggle"` – perjungia LED matricą ON/OFF ir grąžina tekstą `matrix:on` / `matrix:off`;
  - `"/events"` – **SSE** stream su 8×8 temperatūromis;
  - `"/pos"` – grąžina karščiausio taško koordinatės JSON formatu:
    ```json
    { "hx": <0..7>, "hy": <0..7> }
    ```

- **DNS serveris**:
  - veikia kaip **captive portal** – bet koks domenas nukreipiamas į AP IP (pvz. `192.168.4.1`).

---

## 4. Web puslapis (`index.html`)

Tinklalapis saugomas **LittleFS** (`/data/index.html`) ir pateikiamas per `"/"` maršrutą.

Puslapyje yra:

1. **Mygtukas matricos valdymui**  
   - siunčia užklausą į `/toggle`;  
   - šalia rodomas statusas („matrix:on“ / „matrix:off“).

2. **Šilumos žemėlapis (canvas)**  
   - per **SSE** (`/events`) gauna JSON su `temps[64]`;  
   - kiekvienas pikselis atvaizduojamas 8×8 tinklelyje (240×240 px)  
   - spalvos: paprastas mėlyna → raudona gradientas pagal min/max temperatūrą.

3. **Mini žaidimas (antras canvas)**  
   - baltas taškas – žaidėjo „pelytė“, atitinkanti karščiausio taško `hx`, `hy` iš `/pos`;
   - raudonas kvadratas – taikinys atsitiktinėje 8×8 ląstelėje;
   - kai baltas taškas užlipa ant raudono (tas pats `hx`,`hy`) –:
     - taikinys atsiranda naujoje atsitiktinėje pozicijoje;
     - `score` padidinamas +1 ir atnaujinamas ekrane.

---

## 5. Failų ir nustatymų saugojimas

**ESP flash / Preferences**:

- `mx_on` – matricos būsena (ON/OFF);
- `recMaxC` – rekordinė aukščiausia temperatūra;
- `recX`, `recY` – tos rekordinės temperatūros koordinatės 8×8 tinklelyje.

**LittleFS**:

- `index.html` – pagrindinis web puslapis su:
  - 8×8 šilumos žemėlapiu;
  - mini žaidimu (baltas taškas vs. raudonas taikinys);
  - matricos ON/OFF mygtuku.

---

## 6. Papildomas PC skriptas — „termo pelytė“ 

Projekte naudojamas Python skriptas (pvz. `test.py`) kompiuteryje, kuris:

- periodiškai užklausia ESP endpointą **`/pos`** (`http://192.168.4.1/pos`);
- iš JSON paima `hx`, `hy` (karščiausią tašką);
- konvertuoja juos į pelės judėjimą ekrane (naudojant `pyautogui`);
- turi:
  - **centrinę 4×4 zoną** kaip „deadzone“ – kai taškas viduryje, pelė nejuda;
  - **smoothing filtrą** (`ALPHA`, `SCALE`, `INTERVAL`) – kad judėjimas būtų glotnesnis;
  - X krypties apvertimą, kad judėtų „logiškai“.

