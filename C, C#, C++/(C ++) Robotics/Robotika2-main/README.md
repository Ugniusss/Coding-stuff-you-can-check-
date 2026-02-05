<pre>

//Ugnius Padolskis WIFI RSSI checker
  
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <WiFi.h>
#include <Preferences.h>

// ---- PINAI ----
#define BUTTON_PIN 2
#define LED_PIN 7
#define SDA_PIN 4
#define SCL_PIN 5

// ---- LCD 
LiquidCrystal_I2C lcd(0x27, 16, 2);

// ---- TIMER ----
hw_timer_t *timer = NULL;
volatile bool sampleFlag = false;

// ---- NVS (vietoj EEPROM) ----
Preferences prefs;
int mode = 0; // 0 = Live, 1 = Average, 2 = Bars
int avgCount = 0;
float avgRSSI = 0;

// ---- WiFi ----
const char* ssid = "Dlight'as";      
const char* password = "12345678";   

// ---- Kintamieji LED mirksėjimui ----
int32_t lastRSSI = -100;

// ---- ISR ----
void IRAM_ATTR onTimer() {
  sampleFlag = true;
}

void IRAM_ATTR onButton() {
  mode = (mode + 1) % 3;       // dabar 3 režimai
  prefs.putInt("mode", mode);  // išsaugom režimą
}

// ---- SETUP ----
void setup() {
  Serial.begin(115200);

  pinMode(LED_PIN, OUTPUT);
  pinMode(BUTTON_PIN, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(BUTTON_PIN), onButton, FALLING);

  // LCD inicializacija
  Wire.begin(SDA_PIN, SCL_PIN);
  lcd.init();
  lcd.backlight();
  lcd.clear();
  lcd.setCursor(0, 0);
  lcd.print("WiFi RSSI Tracker");
  delay(1000);
  lcd.clear();

  // NVS (Preferences)
  prefs.begin("settings", false);
  mode = prefs.getInt("mode", 0);

  // WiFi jungimas
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  Serial.print("Connecting to WiFi...");
  int retries = 0;
  while (WiFi.status() != WL_CONNECTED && retries < 40) {
    delay(250);
    Serial.print(".");
    lcd.setCursor(0, 0);
    lcd.print("Connecting...");
    retries++;
  }
  lcd.clear();
  if (WiFi.status() == WL_CONNECTED) {
    lcd.print("Connected!");
    Serial.println("\nConnected!");
    Serial.print("IP Address: ");
    Serial.println(WiFi.localIP());
  } else {
    lcd.print("WiFi Failed");
    Serial.println("\nWiFi connect failed!");
  }
  delay(1000);
  lcd.clear();

  // Timeris kas 1 sekundę
  timer = timerBegin(1000000);
  timerAttachInterrupt(timer, &onTimer);
  timerWrite(timer, 0);
  timerAlarm(timer, 1000000, true, 0);
  timerStart(timer);

  Serial.println("System started.");
}

// ---- MAIN LOOP ----
void loop() {
  // Atnaujinam RSSI kas 1s
  if (sampleFlag) {
    sampleFlag = false;
    
    lastRSSI = WiFi.RSSI();
    if (lastRSSI == 0) lastRSSI = -100;

    if (mode == 1) { // AVG režimas
      avgRSSI = (avgRSSI * avgCount + lastRSSI) / (avgCount + 1);
      if (avgCount < 9) avgCount++;
    } else {
      avgRSSI = lastRSSI;
      avgCount = 0;
    }

    // LCD atnaujinimas
    lcd.clear();

    if (mode == 0) {
      // LIVE režimas
      lcd.setCursor(0, 0);
      lcd.print("RSSI:");
      lcd.print(lastRSSI);
      lcd.print("dBm");
      lcd.setCursor(0, 1);
      lcd.print("Mode: Live");
    }
    else if (mode == 1) {
      //AVERAGE režimas
      lcd.setCursor(0, 0);
      lcd.print("RSSI:");
      lcd.print(avgRSSI, 1);
      lcd.setCursor(0, 1);
      lcd.print("Mode: Avg");
    }
    else if (mode == 2) {
      //PADALOS režimas
      lcd.setCursor(0, 0);
      lcd.print("Signal: ");

      // kiek padalų (0–5)
      int bars = 0;
      if (lastRSSI > -45) bars = 5;
      else if (lastRSSI > -55) bars = 4;
      else if (lastRSSI > -65) bars = 3;
      else if (lastRSSI > -75) bars = 2;
      else if (lastRSSI > -85) bars = 1;
      else bars = 0;

      // piešiam padalas
      for (int i = 0; i < bars; i++) lcd.print((char)255);
      for (int i = bars; i < 5; i++) lcd.print(" ");

      lcd.setCursor(0, 1);
      lcd.print("Mode: Bars");
    }

    Serial.print("RSSI: ");
    Serial.print(lastRSSI);
    Serial.print(" dBm | Mode: ");
    if (mode == 0) Serial.println("Live");
    else if (mode == 1) Serial.println("Average");
    else Serial.println("Bars");
  }

  // LED mirksėjimas pagal RSSI (vyksta nuolat)
  if (lastRSSI > -40) {
    digitalWrite(LED_PIN, HIGH); // stiprus – visada šviečia
  } 
  else if (lastRSSI > -60) {
    digitalWrite(LED_PIN, millis() % 500 < 200); // vidutinis – greitas mirksėjimas
  } 
  else {
    digitalWrite(LED_PIN, millis() % 1000 < 200); // silpnas – lėtas mirksėjimas
  }
}
</pre>
