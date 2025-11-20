# MultiFormatReporter

**Scriban** tabanlı PDF, Word ve Excel rapor oluşturma kütüphanesi

## Özellikler

- **PDF Raporları**: HTML template'lerden iText7 kullanarak PDF oluşturma
- **Word Raporları**: HTML template'lerden DocumentFormat.OpenXml ile Word belgeleri oluşturma
- **Excel Raporları**: ClosedXML ile dinamik Excel dosyaları oluşturma
- **Scriban Template Engine**: Güçlü ve esnek template sistemi
- **Dependency Injection Desteği**: .NET Core DI ile kolay entegrasyon
- **Özelleştirilebilir Ayarlar**: Sayfa boyutu, yönlendirme, kenar boşlukları
- **PDF Metadata Desteği**: Başlık, yazar, konu ve anahtar kelimeler

## Kurulum

NuGet paket yöneticisi ile:

```bash
dotnet add package MultiFormatReporter
```

## Gereksinimler

- .NET 9.0 veya üzeri

## Hızlı Başlangıç

### 1. Dependency Injection ile Kullanım

```csharp
using MultiFormatReporter.Extensions;

// Tüm rapor oluşturucuları ekle
services.AddReportGenerators();

// Veya sadece ihtiyaç duyduğun formatları ekle
services.AddPdfReportGenerator();
services.AddWordReportGenerator();
services.AddExcelReportGenerator();
```

### 2. Doğrudan Kullanım

#### PDF Rapor Oluşturma

```csharp
using MultiFormatReporter.Generators;
using MultiFormatReporter.Services;

var pdfGenerator = new PdfReportGenerator(new ScribanTemplateRenderer());

var model = new
{
    title = "Satış Raporu",
    date = DateTime.Now.ToString("dd.MM.yyyy"),
    items = new[]
    {
        new { product = "Laptop", price = 15000 },
        new { product = "Mouse", price = 150 }
    }
};

// Inline template kullanımı
var template = @"
<html>
<head>
    <style>
        body { font-family: Arial; }
        table { border-collapse: collapse; width: 100%; }
        th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
    </style>
</head>
<body>
    <h1>{{ title }}</h1>
    <p>Tarih: {{ date }}</p>
    <table>
        <tr><th>Ürün</th><th>Fiyat</th></tr>
        {{ for item in items }}
        <tr><td>{{ item.product }}</td><td>{{ item.price }} TL</td></tr>
        {{ end }}
    </table>
</body>
</html>";

var pdfBytes = await pdfGenerator.GenerateAsync(template, model);
await File.WriteAllBytesAsync("rapor.pdf", pdfBytes);

// Template dosyasından kullanım
var pdfBytes = await pdfGenerator.GenerateFromFileAsync("template.html", model);
```

#### Word Rapor Oluşturma

```csharp
using MultiFormatReporter.Generators;
using MultiFormatReporter.Services;

var wordGenerator = new WordReportGenerator(new ScribanTemplateRenderer());

var model = new
{
    title = "Çalışan Raporu",
    employees = new[]
    {
        new { name = "Ahmet Yılmaz", department = "IT", salary = 25000 },
        new { name = "Ayşe Demir", department = "İK", salary = 22000 }
    }
};

var wordBytes = await wordGenerator.GenerateFromFileAsync("template.html", model);
await File.WriteAllBytesAsync("rapor.docx", wordBytes);
```

#### Excel Rapor Oluşturma

```csharp
using MultiFormatReporter.Generators;

var excelGenerator = new ExcelReportGenerator();

// Dinamik veriden Excel oluşturma
var data = new[]
{
    new { Urun = "Laptop", Kategori = "Elektronik", Fiyat = 15000, Stok = 45 },
    new { Urun = "Mouse", Kategori = "Aksesuar", Fiyat = 150, Stok = 230 }
};

await excelGenerator.SaveFromDynamicAsync(data, "urunler.xlsx", "Ürün Listesi");

// Özel Excel raporu (ClosedXML workbook ile tam kontrol)
await excelGenerator.SaveCustomAsync(workbook =>
{
    var sheet = workbook.Worksheets.Add("Rapor");

    sheet.Cell("A1").Value = "Başlık";
    sheet.Cell("A1").Style.Font.Bold = true;
    sheet.Cell("A1").Style.Font.FontSize = 16;

    sheet.Cell("A2").Value = "Toplam Satış:";
    sheet.Cell("B2").Value = 1450000;
    sheet.Cell("B2").Style.NumberFormat.Format = "#,##0.00 ₺";

    sheet.Columns().AdjustToContents();

}, "rapor.xlsx");
```

## Yapılandırma

### Rapor Seçenekleri

```csharp
using MultiFormatReporter.Models;

var options = new ReportOptions
{
    PageSize = PageSize.A4,           // A4, A5, Letter, Legal
    Orientation = PageOrientation.Portrait,  // Portrait, Landscape
    Margins = new PageMargins
    {
        Top = 30,
        Right = 30,
        Bottom = 30,
        Left = 30
    }
};

var pdfGenerator = new PdfReportGenerator(
    new ScribanTemplateRenderer(),
    options
);
```

### PDF Metadata

```csharp
var metadata = new PdfMetadata
{
    Title = "Satış Raporu",
    Author = "CSB Sistem",
    Subject = "Aylık Satış Verileri",
    Keywords = "satış, rapor, analiz"
};

var pdfBytes = await pdfGenerator.GenerateAdvancedAsync(template, model, metadata);
```

## Scriban Template Yazma

Scriban, güçlü bir template motorudur. Temel kullanım örnekleri:

### Değişken Yazdırma

```html
<h1>{{ title }}</h1>
<p>Tarih: {{ date }}</p>
```

### Döngüler

```html
<ul>
{{ for item in items }}
    <li>{{ item.name }} - {{ item.price }} TL</li>
{{ end }}
</ul>
```

### Koşullar

```html
{{ if total > 1000 }}
    <p class="high-value">Yüksek değerli sipariş</p>
{{ else }}
    <p>Normal sipariş</p>
{{ end }}
```

### Matematiksel İşlemler

```html
<p>Toplam: {{ quantity * unit_price }} TL</p>
<p>KDV: {{ (subtotal * vat_rate) / 100 }} TL</p>
```

### Tarih Formatlama

```html
<p>{{ date | date.to_string "%d.%m.%Y" }}</p>
<p>{{ date | date.add_days 30 | date.to_string "%d/%m/%Y" }}</p>
```

### Sayı Formatlama

```html
<p>{{ price | math.format "N2" }} TL</p>
```

## Factory Pattern ile Kullanım

```csharp
using MultiFormatReporter;
using MultiFormatReporter.Services;

var factory = new ReportFactory(new ScribanTemplateRenderer());

// PDF oluşturucu al
var pdfGenerator = factory.CreatePdf();
var pdfBytes = await pdfGenerator.GenerateAsync(template, model);

// Word oluşturucu al
var wordGenerator = factory.CreateWord();
var wordBytes = await wordGenerator.GenerateAsync(template, model);

// Excel oluşturucu al
var excelGenerator = factory.CreateExcel();
await excelGenerator.SaveFromDynamicAsync(data, "rapor.xlsx");
```

## Dependency Injection ile Gelişmiş Kullanım

```csharp
// Program.cs veya Startup.cs
services.AddReportGenerators(
    configurePdfOptions: options =>
    {
        options.PageSize = PageSize.A4;
        options.Orientation = PageOrientation.Portrait;
    },
    configureWordOptions: options =>
    {
        options.PageSize = PageSize.Letter;
    }
);

// Controller veya Service içinde
public class ReportService
{
    private readonly PdfReportGenerator _pdfGenerator;

    public ReportService(PdfReportGenerator pdfGenerator)
    {
        _pdfGenerator = pdfGenerator;
    }

    public async Task<byte[]> GenerateInvoice(InvoiceModel invoice)
    {
        var templatePath = "Templates/invoice.html";
        return await _pdfGenerator.GenerateFromFileAsync(templatePath, invoice);
    }
}
```

## Örnek Projeler

`samples/MultiFormatReporter.Sample` klasöründe detaylı örnekler bulabilirsiniz:

- Basit PDF/Word raporları
- Fatura şablonları (Klasik, Modern, Minimal)
- Satış raporları
- Envanter raporları
- Bordro hesap pusulası
- Excel raporları (Basit, Ürün listesi, Özel formatlar)

Örnek projeyi çalıştırmak için:

```bash
cd samples/MultiFormatReporter.Sample
dotnet run
```

## Kullanılan Teknolojiler

- **Scriban** (5.10.0): Template engine
- **iText7** (8.0.5): PDF oluşturma
- **iText7.pdfhtml** (5.0.5): HTML'den PDF dönüşümü
- **DocumentFormat.OpenXml** (3.1.1): Word belgeleri
- **ClosedXML** (0.105.0): Excel dosyaları


