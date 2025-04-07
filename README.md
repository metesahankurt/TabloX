# TabloX - Sanat Eserleri E-Ticaret Platformu

TabloX, sanat eserlerinin online olarak sergilendiği ve satışa sunulduğu bir e-ticaret platformudur. ASP.NET Core MVC kullanılarak geliştirilmiştir.

## Özellikler

- Kullanıcı kaydı ve girişi
- Sanat eseri listeleme ve detay görüntüleme
- Kategori bazlı filtreleme
- Sanatçı profilleri
- Alışveriş sepeti yönetimi
- Sipariş oluşturma ve takip
- Admin paneli ile ürün yönetimi

## Teknolojiler

- ASP.NET Core 6.0
- Entity Framework Core
- SQL Server
- Bootstrap 5
- jQuery
- Identity Framework

## Kurulum

1. Repository'yi klonlayın:
```bash
git clone https://github.com/metesahankurt/TabloX.git
```

2. Veritabanını oluşturun:
```bash
cd TabloX
dotnet ef database update
```

3. Projeyi çalıştırın:
```bash
dotnet run
```

4. Tarayıcınızda şu adresi açın:
```
http://localhost:5001
```

## Veritabanı Yapılandırması

`appsettings.json` dosyasında veritabanı bağlantı dizesini kendi ortamınıza göre güncelleyin:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TabloX;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## Katkıda Bulunma

1. Bu repository'yi fork edin
2. Feature branch'i oluşturun (`git checkout -b feature/YeniOzellik`)
3. Değişikliklerinizi commit edin (`git commit -am 'Yeni özellik eklendi'`)
4. Branch'inizi push edin (`git push origin feature/YeniOzellik`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır. Detaylar için [LICENSE](LICENSE) dosyasına bakın.

## İletişim

Mete Şahan Kurt - [@metesahankurt](https://github.com/metesahankurt) 