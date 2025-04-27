# TabloX

TabloX, dünyaca ünlü sanatçıların eserlerini ve biyografilerini sergileyen, .NET tabanlı bir sanat galerisi web uygulamasıdır. Proje, Metropolitan Museum of Art (MetMuseum) API'sinden alınan açık erişimli görsellerle zenginleştirilmiştir.

## Özellikler
- Sanatçı profilleri ve biyografileri
- Sanat eserlerinin yüksek çözünürlüklü görselleri
- Kategoriye göre filtreleme
- Sipariş ve sepet yönetimi (e-ticaret altyapısı)
- Modern ve kullanıcı dostu arayüz
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
4. **Veritabanını Oluşturun:**
   ```sh
   dotnet ef database update
   ```
5. **Projeyi Başlatın:**
   ```sh
   dotnet run
   ```

## Kullanılan Teknolojiler
- ASP.NET Core
- Entity Framework Core
- MySQL
- MetMuseum API
- Bootstrap

## Katkıda Bulunma
Katkı sağlamak isterseniz, lütfen bir fork oluşturun ve pull request gönderin.

## Lisans
Bu proje MIT lisansı ile lisanslanmıştır.

---

**Not:**
- Proje ilk çalıştırmada otomatik olarak sanatçı ve eser verilerini yükler.
- Herhangi bir sorunla karşılaşırsanız, lütfen bir issue açın veya iletişime geçin. 