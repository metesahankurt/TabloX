# TabloX

TabloX, dünyaca ünlü sanatçıların eserlerini ve biyografilerini sergileyen, .NET tabanlı modern bir sanat galerisi ve e-ticaret web uygulamasıdır. Proje, Metropolitan Museum of Art (MetMuseum) API'sinden alınan açık erişimli görsellerle zenginleştirilmiştir.

## Özellikler
- Sanatçı profilleri ve biyografileri
- Sanat eserlerinin yüksek çözünürlüklü görselleri
- Kategoriye ve sanatçıya göre filtreleme
- Sipariş ve sepet yönetimi (e-ticaret altyapısı)
- Modern ve kullanıcı dostu arayüz (responsive, kırmızı tonlu tema)
- Gelişmiş kullanıcı yönetimi (kayıt, giriş, şifre sıfırlama, 2FA desteği)
- Şifre sıfırlama akışı: Maskelenmiş kullanıcı adı ile güvenli sıfırlama
- E-posta ile şifre sıfırlama ve doğrulama
- Admin paneli
- Veritabanı seed işlemi ile otomatik veri yükleme

## Kurulum
1. **Projeyi Klonlayın:**
   ```sh
   git clone https://github.com/metesahankurt/TabloX.git
   cd TabloX
   ```
2. **Bağımlılıkları Yükleyin:**
   ```sh
   dotnet restore
   ```
3. **Veritabanı Ayarlarını Yapın:**
   - `appsettings.json` dosyasındaki bağlantı bilgisini kendi veritabanınıza göre güncelleyin.
   - E-posta gönderimi için SMTP ayarlarını yapılandırın.
4. **Veritabanını Oluşturun:**
   ```sh
   dotnet ef database update
   ```
5. **Projeyi Başlatın:**
   ```sh
   dotnet run
   ```

## Ekran Görüntüleri
Aşağıda uygulamanın bazı modern arayüz örnekleri yer almaktadır:

| Giriş Ekranı | Şifremi Unuttum | Şifre Sıfırla |
|-------------|-----------------|--------------|
| ![Login](docs/screenshots/login.png) | ![Forgot](docs/screenshots/forgot.png) | ![Reset](docs/screenshots/reset.png) |

## Kullanılan Teknolojiler
- ASP.NET Core MVC
- Entity Framework Core
- MySQL
- MetMuseum API
- Bootstrap 5
- jQuery

## Katkıda Bulunma
Katkı sağlamak isterseniz, lütfen bir fork oluşturun ve pull request gönderin.

## Lisans
Bu proje MIT lisansı ile lisanslanmıştır.

---

**Notlar:**
- Proje ilk çalıştırmada otomatik olarak sanatçı ve eser verilerini yükler.
- Şifre sıfırlama akışı, kullanıcı adı maskelenmesi ve e-posta ile güvenli sıfırlama içerir.
- Herhangi bir sorunla karşılaşırsanız, lütfen bir issue açın veya iletişime geçin. 