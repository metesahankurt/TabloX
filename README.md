# TabloX

TabloX, dünyaca ünlü sanatçıların eserlerini ve biyografilerini sergileyen, .NET tabanlı modern bir sanat galerisi ve e-ticaret web uygulamasıdır. Proje, Metropolitan Museum of Art (MetMuseum) API'sinden alınan açık erişimli görsellerle zenginleştirilmiştir.

## 🎨 Özellikler

### Kullanıcı Yönetimi
- Gelişmiş kayıt ve giriş sistemi
- İki faktörlü kimlik doğrulama (2FA) desteği
- E-posta doğrulama sistemi
- Güvenli şifre sıfırlama akışı
- Kullanıcı profili yönetimi
- Rol tabanlı yetkilendirme (Admin, Kullanıcı)

### Sanat Galerisi
- Sanatçı profilleri ve detaylı biyografileri
- Yüksek çözünürlüklü sanat eseri görselleri
- Kategoriye ve sanatçıya göre filtreleme
- Detaylı eser bilgileri ve açıklamaları
- MetMuseum API entegrasyonu ile otomatik veri yükleme

### E-Ticaret Özellikleri
- Gelişmiş alışveriş sepeti
- Güvenli ödeme sistemi
- Sipariş takibi
- Hediye kartı sistemi
- Sipariş geçmişi
- PDF fatura oluşturma ve indirme

### Admin Paneli
- Kullanıcı yönetimi
- Sanat eseri yönetimi
- Sipariş yönetimi
- İstatistikler ve raporlar
- Sistem ayarları

### Güvenlik
- HTTPS zorunluluğu
- Güvenli çerez politikaları
- CSRF koruması
- XSS koruması
- SQL injection koruması

## 🛠️ Teknolojiler

- **Backend:** ASP.NET Core MVC
- **Veritabanı:** MySQL
- **ORM:** Entity Framework Core
- **Frontend:** 
  - Bootstrap 5
  - jQuery
  - Modern JavaScript
- **API Entegrasyonu:** MetMuseum API
- **E-posta Servisi:** SMTP
- **Kimlik Doğrulama:** ASP.NET Core Identity

## 🚀 Kurulum

1. **Gereksinimler:**
   - .NET 8.0 SDK
   - MySQL Server
   - Git

2. **Projeyi Klonlayın:**
   ```bash
   git clone https://github.com/metesahankurt/TabloX.git
   cd TabloX
   ```

3. **Bağımlılıkları Yükleyin:**
   ```bash
   dotnet restore
   ```

4. **Veritabanı Ayarları:**
   - `appsettings.json` dosyasındaki bağlantı bilgisini güncelleyin
   - E-posta gönderimi için SMTP ayarlarını yapılandırın

5. **Veritabanını Oluşturun:**
   ```bash
   dotnet ef database update
   ```

6. **Projeyi Başlatın:**
   ```bash
   dotnet run
   ```

## 📱 Ekran Görüntüleri

| Giriş Ekranı | Sanat Galerisi | Admin Paneli |
|--------------|----------------|--------------|
| ![Login](/docs/screenshots/login.png) | ![Gallery](/docs/screenshots/gallery.png) | ![Admin](/docs/screenshots/admin.png) |

## 🤝 Katkıda Bulunma

1. Bu projeyi fork edin
2. Yeni bir branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Değişikliklerinizi commit edin (`git commit -m 'Add some amazing feature'`)
4. Branch'inizi push edin (`git push origin feature/amazing-feature`)
5. Bir Pull Request oluşturun

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## 📞 İletişim

Metesahan Kurt - [@metesahankurt](https://github.com/metesahankurt)

Proje Linki: [https://github.com/metesahankurt/TabloX](https://github.com/metesahankurt/TabloX)

## 🙏 Teşekkürler

- Metropolitan Museum of Art - Açık erişimli sanat eserleri için

## 🎯 Tasarım Notları ve Teknik Detaylar

### Mimari Yapı
- **N-Tier Architecture**
  - Presentation Layer (MVC)
  - Business Layer (Services)
  - Data Access Layer (Repositories)
  - Domain Layer (Models)

### Güvenlik Yapılandırması
- **Kimlik Doğrulama**
  - ASP.NET Core Identity entegrasyonu
  - JWT tabanlı token yönetimi
  - Refresh token mekanizması
  - Oturum yönetimi ve timeout politikaları

- **Yetkilendirme**
  - Role-based access control (RBAC)
  - Policy-based authorization
  - Resource-based authorization
  - Custom authorization handlers

### Veritabanı Tasarımı
- **Entity Relationships**
  - One-to-Many: Sanatçı -> Eserler
  - Many-to-Many: Kullanıcılar -> Favoriler
  - One-to-One: Kullanıcı -> Profil

- **Indexing Stratejisi**
  - Performans optimizasyonu için composite indexler
  - Full-text search için özel indexler
  - Foreign key constraintler

### API Tasarımı
- **RESTful Endpoints**
  - Resource-based URL yapısı
  - HTTP metodlarının doğru kullanımı
  - HATEOAS implementasyonu
  - API versiyonlama

### Frontend Mimarisi
- **Component-Based Design**
  - Partial Views
  - View Components
  - Tag Helpers
  - Custom HTML Helpers

- **Client-Side Framework**
  - Bootstrap 5 grid sistemi
  - Responsive tasarım
  - Progressive enhancement
  - Lazy loading

### Performans Optimizasyonları
- **Caching Stratejisi**
  - Response caching
  - Distributed caching
  - Memory caching
  - Output caching

- **Asset Optimization**
  - Bundle ve minification
  - Image optimization
  - CDN entegrasyonu
  - Lazy loading

### Monitoring ve Logging
- **Application Insights**
  - Performance monitoring
  - Error tracking
  - Usage analytics
  - Custom telemetry

- **Logging Stratejisi**
  - Structured logging
  - Log levels
  - Log rotation
  - Error handling

### Deployment Stratejisi
- **CI/CD Pipeline**
  - GitHub Actions
  - Automated testing
  - Deployment automation
  - Environment configuration

- **Containerization**
  - Docker support
  - Multi-stage builds
  - Container orchestration
  - Environment variables

### Test Stratejisi
- **Unit Testing**
  - xUnit framework
  - Mocking
  - Test coverage
  - Integration tests

- **UI Testing**
  - Selenium
  - Playwright
  - Visual regression
  - Accessibility testing
