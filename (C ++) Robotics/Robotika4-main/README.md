<pre>
//Ugnius Padolskis
#include <WiFi.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include <DNSServer.h>
#include <Wire.h>
#include <Adafruit_AMG88xx.h>
#include <Preferences.h>
#include <SPI.h>
#include <MD_MAX72xx.h>
#include <LittleFS.h>

// ---------- PINAI ----------
#define SDA_PIN 8
#define SCL_PIN 9
#define MX_DIN  11
#define MX_CLK  10
#define MX_CS   12
#define BTN_PIN 15

// ----- MAX7219 nustatymai -----
#define HARDWARE_TYPE MD_MAX72XX::FC16_HW
#define MAX_DEVICES   1
MD_MAX72XX mx(HARDWARE_TYPE, MX_DIN, MX_CLK, MX_CS, MAX_DEVICES);

Adafruit_AMG88xx amg;
AsyncWebServer server(80);
AsyncEventSource events("/events");
Preferences prefs;
DNSServer dnsServer;

// Timeris ~10 Hz
hw_timer_t* timer = nullptr;
volatile bool frameFlag = false;
volatile bool btnEdge = false;

bool   matrixEnabled = true;
float  recMaxC = -1000.0f;
uint8_t recX = 0, recY = 0;

volatile uint8_t lastHx = 0;
volatile uint8_t lastHy = 0;

// ---------- FUNKCIJOS ----------
void drawHotspotOnMatrix(uint8_t hx, uint8_t hy) {
  if (!matrixEnabled) {
    mx.clear();
    return;
  }
  mx.clear();
  for (int dy = -1; dy <= 1; dy++) {
    for (int dx = -1; dx <= 1; dx++) {
      int x = hx + dx;
      int y = hy + dy;
      if (x >= 0 && x < 8 && y >= 0 && y < 8) {
        mx.setPoint(y, x, true);
      }
    }
  }
  mx.update();
}

void savePrefs() {
  prefs.begin("cfg", false);
  prefs.putBool("mx_on", matrixEnabled);
  prefs.putFloat("recMaxC", recMaxC);
  prefs.putUChar("recX", recX);
  prefs.putUChar("recY", recY);
  prefs.end();
}

void loadPrefs() {
  prefs.begin("cfg", true);
  matrixEnabled = prefs.getBool("mx_on", true);
  recMaxC = prefs.getFloat("recMaxC", -1000.0f);
  recX = prefs.getUChar("recX", 0);
  recY = prefs.getUChar("recY", 0);
  prefs.end();
}

// Pertraukimai
void IRAM_ATTR onTimer() { frameFlag = true; }
void IRAM_ATTR onButton() { btnEdge = true; }

void setup() {
  Serial.begin(115200);
  delay(50);

  // LittleFS
  if (!LittleFS.begin(true)) {
    Serial.println("Nepavyko LittleFS");
  } else {
    Serial.println("LittleFS paleistas");
  }

  // MAX7219
  mx.begin();
  mx.control(MD_MAX72XX::SHUTDOWN, MD_MAX72XX::OFF);
  mx.control(MD_MAX72XX::INTENSITY, 8);
  mx.clear();

  loadPrefs();
  if (!matrixEnabled) mx.clear();

  // AMG8833
  Wire.begin(SDA_PIN, SCL_PIN);
  if (!amg.begin()) {
    Serial.println("AMG8833 nerasta!");
    while (1) { delay(1000); }
  }

  // Mygtukas – matrica ON/OFF
  pinMode(BTN_PIN, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(BTN_PIN), onButton, FALLING);

  // Wi-Fi 
  WiFi.mode(WIFI_AP);
  WiFi.softAP("SilumosWeb");
  IPAddress apIP = WiFi.softAPIP();
  dnsServer.start(53, "*", apIP);

  // Pagrindinis puslapis
  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send(LittleFS, "/index.html", "text/html; charset=utf-8");
  });

  
  const char* redirects[] = {
    "/generate_204", "/gen_204", "/hotspot-detect.html",
    "/library/test/success.html", "/ncsi.txt", "/fwlink"
  };
  for (auto path : redirects) {
    server.on(path, HTTP_GET, [](AsyncWebServerRequest *request){
      request->redirect("/");
    });
  }

  // Web mygtukas matricai
  server.on("/toggle", HTTP_GET, [](AsyncWebServerRequest *request){
    matrixEnabled = !matrixEnabled;
    if (!matrixEnabled) mx.clear();
    savePrefs();
    request->send(200, "text/plain; charset=utf-8",
                  matrixEnabled ? "matrix:on" : "matrix:off");
  });


  server.on("/pos", HTTP_GET, [](AsyncWebServerRequest *request){
    uint8_t fx = 7 - lastHx;   
    String json = "{";
    json += "\"hx\":"; json += fx;          
    json += ",\"hy\":"; json += (uint8_t)lastHy;
    json += "}";
    request->send(200, "application/json", json);
  });


  server.addHandler(&events);
  server.begin();

  // 10 Hz timeris
  timer = timerBegin(1000000);
  timerAttachInterrupt(timer, &onTimer);
  timerAlarm(timer, 100000, true, 0);
  timerStart(timer);

  Serial.print("Start SilumosWeb  IP: ");
  Serial.println(apIP);
}

void loop() {
  dnsServer.processNextRequest();

  // Mygtuko debouncing
  static uint32_t lastBtnMs = 0;
  if (btnEdge) {
    btnEdge = false;
    uint32_t now = millis();
    if (now - lastBtnMs > 150) {
      lastBtnMs = now;
      matrixEnabled = !matrixEnabled;
      if (!matrixEnabled) mx.clear();
      savePrefs();
    }
  }

  if (!frameFlag) {
    vTaskDelay(1);
    return;
  }
  frameFlag = false;

  // Thermo sensorius
  float pix[64];
  amg.readPixels(pix);

  int hiIdx = 0;
  float hiVal = pix[0];
  for (int i = 1; i < 64; i++) {
    if (pix[i] > hiVal) {
      hiVal = pix[i];
      hiIdx = i;
    }
  }

  uint8_t hx = hiIdx % 8;
  uint8_t hy = hiIdx / 8;

  lastHx = hx;
  lastHy = hy;

  drawHotspotOnMatrix(hx, hy);

  // rekordinė temp
  if (hiVal > recMaxC + 0.05f) {
    recMaxC = hiVal;
    recX = hx;
    recY = hy;
    savePrefs();
  }

  // JSON web'ui
  String json = "{\"temps\":[";
  for (int i = 0; i < 64; i++) {
    json += String(pix[i], 1);
    if (i < 63) json += ",";
  }
  json += "]}";
  events.send(json.c_str());
}
</pre>
