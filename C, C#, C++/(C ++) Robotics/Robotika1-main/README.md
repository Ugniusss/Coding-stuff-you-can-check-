#include <Adafruit_LiquidCrystal.h>
#include <Servo.h>

Adafruit_LiquidCrystal lcd_dist(7);  // Distance display
Adafruit_LiquidCrystal lcd_servo(6); // Servo angle display

Servo myServo;

// RGB LED pins 
const int redPin = 3;
const int greenPin = 5;
const int bluePin = 6;

long readUltrasonicDistance(int triggerPin, int echoPin) {
  pinMode(triggerPin, OUTPUT);  
  digitalWrite(triggerPin, LOW);
  delayMicroseconds(2);
  digitalWrite(triggerPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(triggerPin, LOW);
  pinMode(echoPin, INPUT);
  return pulseIn(echoPin, HIGH);
}

void setup() {
  lcd_dist.begin(16, 2);
  lcd_servo.begin(16, 2);
  
  myServo.attach(9); 

  pinMode(redPin, OUTPUT);
  pinMode(greenPin, OUTPUT);
  pinMode(bluePin, OUTPUT);
}

void loop() {
  // --- ATSTUMAS
  lcd_dist.clear();
  lcd_dist.setCursor(0, 0);
  lcd_dist.print("Atstum : ");
  float distance = 0.01723 * readUltrasonicDistance(13, 12);
  lcd_dist.print(distance);
  lcd_dist.print(" cm");

  // --- SERVO pirmyn
  for (int angle = 0; angle <= 180; angle += 30) {
    myServo.write(angle);
    setLEDColor(angle);
    lcd_servo.clear();
    lcd_servo.setCursor(0, 0);
    lcd_servo.print("Servo Deg:");
    lcd_servo.setCursor(0, 1);
    lcd_servo.print(angle);
    delay(1000);
  }

  // --- SERVO atgal
  for (int angle = 180; angle >= 0; angle -= 30) {
    myServo.write(angle);
    setLEDColor(angle);
    lcd_servo.clear();
    lcd_servo.setCursor(0, 0);
    lcd_servo.print("Servo Deg:");
    lcd_servo.setCursor(0, 1);
    lcd_servo.print(angle);
    delay(1000);
  }
}


void setLEDColor(int angle) {
  if(angle <= 60){
    // Red
    analogWrite(redPin, 255);
    analogWrite(greenPin, 0);
    analogWrite(bluePin, 0);
  } else if(angle <= 120){
    // Green
    analogWrite(redPin, 0);
    analogWrite(greenPin, 255);
    analogWrite(bluePin, 0);
  } else {
    // Blue
    analogWrite(redPin, 0);
    analogWrite(greenPin, 0);
    analogWrite(bluePin, 255);
  }
}
