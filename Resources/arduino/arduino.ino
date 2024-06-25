#define PIN_OgrenciZili 4
#define PIN_OgretmenZili 5
#define PIN_CikisZili 6
#define LED_BUILTIN 17

int currentState = 0;

void setup() {
  // Seri port başlat
  Serial.begin(9600);

  // Pin modları ayarla
  pinMode(PIN_OgrenciZili, OUTPUT);
  pinMode(PIN_OgretmenZili, OUTPUT);
  pinMode(PIN_CikisZili, OUTPUT);
  pinMode(LED_BUILTIN, OUTPUT);

  // Pinleri LOW yaparak başla
  digitalWrite(PIN_OgrenciZili, LOW);
  digitalWrite(PIN_OgretmenZili, LOW);
  digitalWrite(PIN_CikisZili, LOW);
  digitalWrite(LED_BUILTIN, LOW);
}

void loop() {
  // Seri port üzerinden veri gelmesini bekle
  if (Serial.available()) {
    String input = Serial.readStringUntil('\n');

    // Gelen mesajı kontrol et
    int targetState = input.toInt();

      // Önceki pini LOW yap
      switch (currentState) {
        case 4:
          digitalWrite(PIN_OgrenciZili, LOW);
          Serial.println("Ogrenci Zili: LOW");
          break;

        case 5:
          digitalWrite(PIN_OgretmenZili, LOW);
          Serial.println("Ogretmen Zili: LOW");
          break;

        case 6:
          digitalWrite(PIN_CikisZili, LOW);
          Serial.println("Cikis Zili: LOW");
          break;
      }

      // Yeni durumu HIGH yap
      switch (targetState) {
        case 4:
          digitalWrite(PIN_OgrenciZili, HIGH);
          Serial.println("Ogrenci Zili: HIGH");
          blinkLED(targetState);
          break;

        case 5:
          digitalWrite(PIN_OgretmenZili, HIGH);
          Serial.println("Ogretmen Zili: HIGH");
          blinkLED(targetState);
          break;

        case 6:
          digitalWrite(PIN_CikisZili, HIGH);
          Serial.println("Cikis Zili: HIGH");
          blinkLED(targetState);
          break;
      }

      // Güncel durumu kaydet
      currentState = targetState;
  }
}

void blinkLED(int times) {
  for (int i = 0; i < times; i++) {
    digitalWrite(LED_BUILTIN, HIGH);
    delay(100);  // 100 ms bekle
    digitalWrite(LED_BUILTIN, LOW);
    delay(100);  // 100 ms bekle
  }
}