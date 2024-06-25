### Otomatik Okul Zili Programı - Kullanım Kılavuzu

**Amaç:** Belirli zamanlarda otomatik olarak ilgili zili çalmak ve Arduino tabanlı bir sisteme zil sinyali göndermek.

#### Kurulum
0. **İndirme:** Sıkıştırılmış dosyayı, programın dizini olacak yere açın.
1. **Başlangıçt Çalıştırma:** Program başlangıçta çalışması için "Shell:Startup" yoluna kısayol eklenmelidir.
2. **Arduino Bağlantısı:** Arduino'nun bilgisayara doğru şekilde bağlandığını kontrol edin, kendisi bağlanır.

#### Arayüz Kullanımı
- **Zamanlama Ayarları:**
  - Sabahçı ve öğlenci zil saatleri için zamanlayıcıları ayarlayın.
  - Zil türlerini ve ses seviyelerini belirleyin.
  
- **Ses Ayarları:**
  - Hazır zil seslerinden seçim yapın.
  - Dinle butonuyla sesi kontrol edebilir, durdur butonuyla kesebilirsiniz.
  
#### Dosyalar ve Klasörler
- **BellSchedule.json:** Zamanlama bilgilerini saklar.
- **Sounds klasörü:** Zil ses dosyalarını içerir.
- **Logs klasörü:** Günlük kayıtlarını tutar.

#### Günlük Kayıtlar
- Her zil çalındığında ve Arduino bağlantı durumu değiştiğinde otomatik olarak günlük kayıt tutulur. ```Logs``` klasöründe ```day_yyyy-MM-dd.log``` formatında saklanır.

#### Program bildirim çubuğunda (gizli) açılabilir.
