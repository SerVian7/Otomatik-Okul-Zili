#define PIN_OgrenciZili 4
#define PIN_OgretmenZili 5
#define PIN_CikisZili 6

void setup() {
  // Seri port başlat
  Serial.begin(9600);

  // Pin modları ayarla
  pinMode(PIN_OgrenciZili, OUTPUT);
  pinMode(PIN_OgretmenZili, OUTPUT);
  pinMode(PIN_CikisZili, OUTPUT);

  // Pinleri LOW yaparak başla
  digitalWrite(PIN_OgrenciZili, LOW);
  digitalWrite(PIN_OgretmenZili, LOW);
  digitalWrite(PIN_CikisZili, LOW);
}

void loop() {
  // Seri port üzerinden veri gelmesini bekle
  if (Serial.available()) {
    int state = Serial.parseInt();

    // Gelen değere göre ilgili pini HIGH yap
    switch (state) {
      case 4:
        digitalWrite(PIN_OgrenciZili, HIGH);
        Serial.println("Ogrenci Zili: HIGH");
        delay(1000); // 1 saniye bekle
        digitalWrite(PIN_OgrenciZili, LOW);
        Serial.println("Ogrenci Zili: LOW");
        break;

      case 5:
        digitalWrite(PIN_OgretmenZili, HIGH);
        Serial.println("Ogretmen Zili: HIGH");
        delay(1000); // 1 saniye bekle
        digitalWrite(PIN_OgretmenZili, LOW);
        Serial.println("Ogretmen Zili: LOW");
        break;

      case 6:
        digitalWrite(PIN_CikisZili, HIGH);
        Serial.println("Cikis Zili: HIGH");
        delay(1000); // 1 saniye bekle
        digitalWrite(PIN_CikisZili, LOW);
        Serial.println("Cikis Zili: LOW");
        break;

      default:
        // Bilinmeyen durum
        Serial.println("Unknown state received");
        break;
    }
  }
}