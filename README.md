### Otomatik Okul Zili Programı - Kullanım Kılavuzu
**Amaç:** Belirli zamanlarda otomatik olarak ilgili zili çalmak ve Arduino tabanlı bir sisteme zil sinyali göndermek.

![image](https://github.com/zengeragency/Otomatik-Okul-Zili/assets/62677804/39759396-3398-45b5-be8b-7d9f6c6c0d56)
![image](https://github.com/zengeragency/Otomatik-Okul-Zili/assets/62677804/bdd0fd97-5a23-477b-9e36-dd20d1c35bdf)

#### Kurulum
0. **İndirme:** Sıkıştırılmış dosyayı, programın dizini olacak yere açın.
1. **Başlangıçts Çalıştırma:** Program başlangıçta çalışması için "Shell:Startup" yoluna kısayol eklenmelidir.
2. Arduino kodu "/Resources" dizinin altındadır.
3. **Arduino Bağlantısı:** Arduino'nun bilgisayara doğru şekilde bağlandığını kontrol edin, kendisi bağlanır.

#### Program bildirim çubuğunda çalışıyor. Kapatmak için bildirim çubuğundaki ikona sağ tılkayıp "Programı Kapat" seçeneğine basın.

#### Arayüz Kullanımı
- **Zamanlama Ayarları:**
  - Sabahçı ve öğlenci zil saatleri için zamanlayıcıları ayarlayın.
  - Zil türlerini ve ses seviyelerini belirleyin.
  
- **Ses Ayarları:**
  - Hazır zil seslerinden seçim yapın.
  - Dinle butonuyla sesi kontrol edebilir, durdur butonuyla kesebilirsiniz.

#### Günlük Kayıtlar
- Her zil çalındığında ve Arduino bağlantı durumu değiştiğinde otomatik olarak günlük kayıt tutulur. ```Logs``` klasöründe ```day_yyyy-MM-dd.log``` formatında saklanır.
  
#### Dosyalar ve Klasörler
- **BellSchedule.json:** Zamanlama bilgilerini saklar.
- **Sounds klasörü:** Zil ses dosyalarını içerir.
- **Logs klasörü:** Günlük kayıtlarını tutar.
