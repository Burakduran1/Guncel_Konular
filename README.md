# Dünya Kaşifi

Artırılmış Gerçeklik Temelli Mobil Uygulama

<p align="center">
  <img src="./dokümanlar/app_logo.png" alt="Dünya Kaşifi Logo" width="200"/>
</p>

## 📱 Proje Açıklaması

Dünya Kaşifi, çocukların uçak yolculukları sırasında eğlenceli ve eğitici bir deneyim yaşamalarını sağlamak amacıyla tasarlanmış artırılmış gerçeklik temelli bir mobil uygulamadır. Uygulama, çocukların kişiselleştirilmiş avatarları ile interaktif bir keşif macerasına çıkmasını sağlar.

## 🌟 Proje Özellikleri

### a) Oyuna Giriş ve Karakter Oluşturma
- **Avatar Tasarımı:** Çocuk, saç stili, göz rengi, kıyafet ve aksesuar seçerek kendi avatarını oluşturabilir.
- **Keşif Ekipmanları Seçimi:** Sanal dürbün, sihirli pusula, not defteri ve fotoğraf makinesi gibi araçları seçebilir.
- **Favori Araç Belirleme:** Macerada kullanılacak sihirli halı, küçük uçak, roket veya sıcak hava balonu gibi araçlardan seçim yapılır.
- **Kaşif Sertifikası:** Eğlenceli "Kaşif Yemini" sonrasında dijital "Dünya Kaşifi Sertifikası" verilir.

### b) Uçuş Rotası Tanıtımı
- **Renkli Duraklar:** Geçilecek ülkeler ve şehirler, ilgi çekici ikonlarla işaretlenir.
- **Hedefler ve Ödüller:** Her durak noktasında kazanılabilecek rozetler ve ödüller tanıtılır.
- **Görev Listesi:** Uçuş sürecinde tamamlanabilecek görevler detaylandırılır.

### c) İnteraktif Görevler ve Mini Oyunlar
- **Coğrafi Keşifler:** Ülkelerin 3D haritaları, ünlü yapılar (örneğin Eyfel Kulesi, Kolezyum, Big Ben) ve mini "Harita Dedektifi" oyunu ile gizli lokasyonların bulunması; ayrıca bilgi kartları.
- **Kültürel Maceralar:** Geleneksel kıyafetlerin avatar üzerine giydirilmesi, yöresel yemeklerin toplanması ve tanınması, kültürel semboller eşleştirme görevleri.
- **Dil Öğrenme Aktiviteleri:** Ülkelerin temel selamlaşma ifadeleri, sayı ve renk gibi kelimeler ile telaffuz oyunları.
- **Pencere Görünümü Zenginleştirme:** AR teknolojisi ile bulutların üzerinde uçan hayvanlar, önemli yapılar ve bulut boyama/şekil oluşturma oyunları.
- **Mini Oyunlar:** "Gökyüzü Matematik Yarışması" ile uçuş yüksekliği ve mesafe problemleri, "Hava Durumu Tahmincisi" oyunu.

### d) Eğitim ve İlerleme Sistemi
- **Kaşif Pasaportu:** Her ülke için özel damgalar ve dil pulları; tamamlanan görevler ve öğrenilen bilgiler pasaporta işlenir.
- **Ödül Mekanizması:** "Süper Kaşif" seviye sistemi ile ilerleme motive edilir. Dijital hatıralar ve avatar aksesuarları ödül olarak sunulur.

### e) Güvenlik ve Konfor Özellikleri
- **Göz Sağlığı:** 20 dakikada bir göz dinlendirme hatırlatıcıları.
- **Doğru Pozisyon:** Oturma pozisyonu animasyonları ve öneriler.
- **Ebeveyn Kontrolü:** İçerik filtreleme, süre kontrolü ve ilerleme raporları.

### f) Uçuş Sonu Deneyimi
- **Dijital Seyahat Günlüğü:** Uçuş boyunca kazanılan başarılar özetlenir.
- **Yeni Hedefler:** Bir sonraki uçuş için yeni hedefler önerilir.
- **Macera Sınavı:** Kısa bir sınav ile eğlenceli bir kapanış yapılır.

## 🔧 Kurulum ve Çalıştırma

### APK İndirme ve Kurulum
Uygulamamızın en son sürümünü indirmek için:
1. [Dünya Kaşifi APK (v1.0.0)](./releases/dunya_kasifi_v1.0.0.apk) dosyasını indirin
2. Android cihazınızda "Bilinmeyen Kaynaklardan Yükleme"yi etkinleştirin (Ayarlar > Güvenlik)
3. İndirilen APK dosyasını açın ve kurulum talimatlarını izleyin
4. Kurulum tamamlandıktan sonra, uygulamayı başlatın ve maceraya başlayın!

### Geliştirici Kurulumu
Projeyi geliştirici olarak yerel ortamınızda çalıştırmak için aşağıdaki adımları izleyin:

```bash
# Repoyu klonlayın
git clone https://github.com/dunya-kasifi/dunya-kasifi-ar.git
cd dunya-kasifi-ar

# Sanal ortam oluşturma
python -m venv venv

# Sanal ortamı aktifleştirme (Windows)
venv\Scripts\activate

# Gerekli kütüphaneleri yükleme
pip install -r requirements.txt

# Unity projesi için gerekli bağımlılıkları yükleme
cd unity_project
# Unity Hub'dan projeyi açabilirsiniz

# Uygulamayı başlatma 
python app.py
```

### Minimum Sistem Gereksinimleri
- **Android:** Android 8.0 ve üzeri
- **İşlemci:** Snapdragon 660 veya eşdeğeri
- **RAM:** 4 GB
- **Depolama:** 500 MB boş alan
- **Sensörler:** Gyroscope, Kamera

## 📚 Proje Dokümanları

### Kullanıcı Kılavuzları
- [Kullanım Kılavuzu](./dokümanlar/kullanim_kilavuzu.pdf) - Uygulamanın kullanımı için detaylı açıklamalar
- [Hızlı Başlangıç Rehberi](./dokümanlar/hizli_baslangic.pdf) - Yeni kullanıcılar için adım adım başlangıç
- [Ebeveyn Kontrol Kılavuzu](./dokümanlar/ebeveyn_kilavuzu.pdf) - Ebeveynler için kontrol ve takip özellikleri
- [Sık Sorulan Sorular](./dokümanlar/sss.pdf) - Yaygın sorunlar ve çözümleri

### Teknik Diyagramlar

- [UML Use Case Diyagramı](./Diagrams/USE_CASE.svg) - Kullanıcı etkileşimlerini gösterir
- [Sınıf Diyagramı](./Diagrams/Sınıf_Diyagramı.svg) - Uygulamanın nesne yapısını gösterir
- [Aktivite Diyagramı](./Diagrams/Activity.svg) - Uygulama içi aktivite akışını gösterir
- [Nesne Diyagramı](./Diagrams/Object.svg) - Çalışma anında nesnelerin durumunu gösterir
- [Sıralama Diyagramı](./Diagrams/Sequence%20.svg) - Sistem bileşenleri arasındaki etkileşimleri gösterir
- [Durum Diyagramı](./Diagrams/State.svg) - Uygulama durumlarını ve geçişlerini gösterir
- [Bileşen Diyagramı](./Diagrams/Component.svg) - Uygulama bileşenlerini ve bağımlılıklarını gösterir
- [Deployment Diyagramı](./Diagrams/Deployment.svg) - Dağıtım mimarisini gösterir
- [Gantt Şeması](./dokümanlar/Gantt_Semması.png) - Proje zaman çizelgesini gösterir

### Proje Analiz Dokümanları

- [SMART Analizi](./dokümanlar/SMART.docx) - Proje hedeflerinin Specific, Measurable, Achievable, Relevant, Time-bound analizi
- [SWOT Analizi](./dokümanlar/SWOT_Analizi.docx) - Güçlü yönler, zayıf yönler, fırsatlar ve tehditler analizi
- [Gereksinim Analizi](./dokümanlar/Gereksinim_Analizi.docx) - Fonksiyonel ve fonksiyonel olmayan gereksinimler

## 🧩 Kod Yapısı ve Artırılmış Gerçeklik Özellikleri

Dünya Kaşifi, Unity 3D ve ARCore/ARKit teknolojileri kullanılarak geliştirilmiş bir artırılmış gerçeklik uygulamasıdır.

### Kod Organizasyonu
```
src/
├── ar/                    # AR alt yapısı ve özellikleri
│   ├── ARManager.cs       # Ana AR sistemi yöneticisi
│   ├── CloudRecognition.cs # Bulut tabanlı AR izleme sistemi
│   └── GeoAR.cs           # Coğrafi konum tabanlı AR özellikleri
├── ui/                    # Kullanıcı arayüzü bileşenleri
│   ├── AvatarCreator.cs   # Avatar oluşturma sistemi
│   └── MapInterface.cs    # Harita arayüzü ve etkileşimleri
├── game/                  # Oyun mekanikleri
│   ├── MiniGames.cs       # Mini oyun sistemi
│   ├── Quests.cs          # Görev sistemi
│   └── Rewards.cs         # Ödül ve rozet sistemi
└── utils/                 # Yardımcı sınıflar ve fonksiyonlar
    ├── DataManager.cs     # Veri yönetimi
    └── SafetyFeatures.cs  # Güvenlik özellikleri
```

### Artırılmış Gerçeklik Özellikleri

Uygulamamız aşağıdaki AR teknolojilerini kullanır:

1. **Görüntü İzleme (Image Tracking):** Haritalar ve kartlar üzerinde 3D modeller gösterme
2. **Düzlem Algılama (Plane Detection):** Uçak penceresi dışındaki dünya üzerinde sanal nesneler yerleştirme
3. **Yüz İzleme (Face Tracking):** Avatar kişiselleştirme ve yüz filtrelerini uygulama
4. **Coğrafi Konum AR (Geo AR):** Uçuş rotasında gerçek lokasyonlara bağlı içerikler gösterme
5. **Bulut Şekillendirme:** AR ile gökyüzündeki bulutları interaktif olarak şekillendirme ve boyama

## 📹 Demo ve Tanıtım Videoları

- [Dünya Kaşifi Tanıtım Videosu](https://www.youtube.com/watch?v=example1) - Uygulamanın genel tanıtımı
- [Kurulum ve Kullanım Rehberi](https://www.youtube.com/watch?v=example2) - Adım adım kurulum ve kullanım
- [AR Özellikleri Demo](https://www.youtube.com/watch?v=example3) - Artırılmış gerçeklik özelliklerinin demosu
- [Geliştirici Günlüğü #1](https://www.youtube.com/watch?v=example4) - Geliştirme sürecinin arkasındaki hikaye


## 📋 Proje Takibi ve Yol Haritası

Projeye ait tüm görevler, gelişim süreci ve sorumluluk paylaşımı Trello üzerinden detaylı olarak planlanmıştır. Aşağıdaki bağlantı üzerinden projemizin aşamalarını adım adım takip edebilirsiniz:

🔗 [Trello Proje Panosu – Kalite Proje](https://trello.com/b/8Xt6CWoD/d%C3%BCnya-ka%C5%9Fifi)

### 🗂️ Trello Panosundaki Bölümler

| Başlık | Açıklama |
|--------|----------|
| 📌 **Yapılacaklar (To Do)** | Henüz başlanmamış görevler ve planlanan işler |
| 🚧 **Devam Edenler (In Progress)** | Şu anda üzerinde çalışılan görevler |
| ✅ **Tamamlananlar (Done)** | Tamamlanmış tüm görev ve aşamalar |
| 🧠 **Araştırma / Analiz** | Projeye dair ön araştırmalar, referanslar ve teknik analizler |
| 🎨 **Tasarım** | Uygulamanın arayüz tasarımları ve kullanıcı deneyimi planları |
| 💻 **Geliştirme** | Unity ve ARCore/ARKit ile yapılan kodlama süreçleri |
| 🧪 **Test ve Geri Bildirim** | Test süreci, kullanıcı geri bildirimleri ve hata düzeltmeleri |
| 🗣️ **Sunum Hazırlığı** | Proje tanıtımı, sunum slaytları ve video içerikler |
| 📦 **Teslim** | Final dökümanları, video bağlantıları ve proje teslim dosyaları |

Trello panosu, proje yönetim sürecimizi daha şeffaf ve organize bir şekilde yürütmemize yardımcı olmaktadır. Her takım üyesi görev dağılımını, son teslim tarihlerini ve ilerleme durumunu bu pano üzerinden kolayca takip edebilir.


## 👨‍💻 Geliştirme Ekibi

- Dünya Kaşifi Proje Ekibi
  - AR Geliştirme Ekibi
  - UI/UX Tasarım Ekibi
  - Eğitici İçerik Geliştirme Ekibi
  - Test ve Kalite Kontrol Ekibi

## 📜 Lisans

Bu proje [MIT lisansı](LICENSE) altında lisanslanmıştır.

