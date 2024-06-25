### Otomatik Okul Zili Programı - Kullanım Kılavuzu

#### Özellikler:
- **Zil Çalma Zamanlaması:** Öğrenci zili, öğretmen zili ve ders sonu zili için zamanları kolayca ayarlayabilirsiniz.
- **Arduino Bağlantısı:** Program, otomatik olarak Arduino'yu tanıyıp, duruma göre komutlar gönderebilir.
- **Ses Seçimi ve Dinleme:** Hazır zil seslerinden seçim yapabilir ve anında dinleyebilirsiniz.
- **Günlük Kayıt Tutma:** Her gün için ayrıntılı bir günlük kaydı tutar.
- **Gizli Çalışma:** Programı gizli modda çalıştırabilir ve sistem çubuğundan kontrol edebilirsiniz.

#### Kurulum ve Başlatma:
1. **Kurulum Dosyaları:**
   - **BellSchedule.json:** Zamanlama bilgilerini saklayan dosya.
   - **Sounds klasörü:** Zil ses dosyalarının saklandığı klasör.
   - **Logs klasörü:** Günlük kayıtlarının tutulduğu klasör.

2. **Başlatma:**
   - Programın başlangıçta otomatik olarak çalışması için çalıştırılabilir dosyayı sistem başlangıç yoluna ekleyin.

#### Arayüz:
- **Sabahçı ve Öğlenci Zil Ayarları:**
  - Zamanlayıcı alanında sabahçı ve öğlenci zil saatlerini belirleyebilirsiniz.
  
- **Ses Ayarları:**
  - Hazır ses dosyalarını seçebilir ve zil saatleri için farklı sesler belirleyebilirsiniz.
  - Ses dosyasını Listen butonuyla dinleyip, durdurabilirsiniz.

- **Ses Kontrol:**
  - Her zil türü için ayrı ses seviyesi belirleyebilirsiniz.

#### Kullanımı:
- **Zil Ayarları:** Sabahçı ve öğlenci zil saatlerini belirleyin, zil türünü ve ses seviyesini seçin.
- **Arduino Bağlantısı:** Program Arduino'yu otomatik olarak tanır ve bağlanır. Bağlantı durumu arayüzde gösterilir.
- **Dinleme Modu:** ``Dinle`` butonuna tıklayarak seçili sesi dinleyebilir, ``Durdur`` butonuyla durdurabilirsiniz.
- **Gizli Mod:** Programı gizleyerek sistem tepsisinde çalıştırabilir, tepsi simgesinden programı kontrol edebilirsiniz.

#### Günlük Kayıt:
- **Kayıt Dosyaları:** Logs klasöründe her gün için ``day_yyyy-MM-dd.log`` dosyaları oluşturulur.
- **İçerik:** Her zil çalındığında ve Arduino bağlantı durumu değiştiğinde otomatik olarak günlük kaydı tutulur.
