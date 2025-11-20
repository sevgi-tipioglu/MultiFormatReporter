# 📚 SCRIBAN TEMPLATE KÜTÜPHANESİ

CSB.Module.Report için hazır Scriban template koleksiyonu.

## 📁 Klasör Yapısı

```
Templates/
├── Shared/
│   └── common-styles.css          # Ortak CSS stilleri
├── Invoices/
│   ├── classic-invoice.html       # Klasik/Profesyonel fatura
│   ├── modern-invoice.html        # Modern/Renkli fatura
│   └── minimal-invoice.html       # Minimal/Temiz fatura
├── Reports/
│   ├── sales-report.html          # Satış performans raporu
│   └── inventory-report.html      # Stok durum raporu
├── Payroll/
│   └── monthly-payroll.html       # Aylık maaş bordrosu
└── README.md                       # Bu dosya
```

---

## 🎨 TEMPLATE KATALOU

### 💰 Fatura Şablonları (Invoices)

#### 1. **classic-invoice.html** - Klasik Fatura
**Stil:** Profesyonel, geleneksel
**Renk Paleti:** Lacivert (#2c3e50), Gri tonu
**Kullanım Senaryosu:** B2B faturalar, resmi belgeler

**Gerekli Model Alanları:**
```csharp
{
    invoice_number,          // Fatura numarası
    invoice_date,            // Fatura tarihi
    due_date (optional),     // Vade tarihi
    payment_status,          // "Ödendi" veya "Ödenmedi"
    currency,                // "TL", "USD", "EUR"
    seller {                 // Satıcı bilgileri
        company_name,
        address, city, country,
        tax_number, phone, email
    },
    customer {               // Alıcı bilgileri
        name, address, city, country,
        tax_number (optional),
        phone, email
    },
    line_items [{            // Ürün/hizmet listesi
        product,
        description (optional),
        quantity,
        unit_price
    }],
    subtotal,                // Ara toplam
    discount (optional),     // İndirim tutarı
    discount_rate (optional),// İndirim oranı
    vat_rate,                // KDV oranı
    vat_amount,              // KDV tutarı
    total,                   // Genel toplam
    notes (optional),        // Notlar
    payment_terms (optional) // Ödeme koşulları
}
```

**Örnek Kullanım:**
```csharp
var pdfGen = ReportFactory.CreatePdf();
await pdfGen.GenerateFromFileAsync(
    TemplateManager.Invoices.Classic,
    model
);
```

---

#### 2. **modern-invoice.html** - Modern Fatura
**Stil:** Renkli gradyanlar, modern UI
**Renk Paleti:** Mor-Mavi gradient (#667eea → #764ba2)
**Kullanım Senaryosu:** Teknoloji şirketleri, kreatif ajanslar

**Özellikler:**
- ✅ Gradient header
- ✅ Kart tabanlı bilgi kutuları
- ✅ Emoji ikonlar
- ✅ Renkli durum badge'leri

---

#### 3. **minimal-invoice.html** - Minimal Fatura
**Stil:** Minimalist, temiz, sade
**Renk Paleti:** Siyah-Beyaz, minimal renkler
**Kullanım Senaryosu:** Freelancer'lar, boutique firmalar

**Özellikler:**
- ✅ İnce çizgiler
- ✅ Bol boşluk kullanımı
- ✅ Tipografi odaklı
- ✅ İngilizce etiketler (international)

---

### 📊 Rapor Şablonları (Reports)

#### 4. **sales-report.html** - Satış Raporu
**Kullanım:** Aylık/dönemsel satış performans analizi
**Sayfa Yönü:** Landscape önerilir

**Gerekli Model:**
```csharp
{
    report_title,
    company_name,
    period_start, period_end,
    currency,
    summary {
        total_sales,         // Toplam satış adedi
        total_revenue,       // Toplam gelir
        average_order,       // Ortalama sipariş
        total_customers      // Müşteri sayısı
    },
    performance_metrics [{ // Performans metrikleri
        name, current, previous,
        change,              // Değişim yüzdesi
        progress,            // İlerleme barı (0-100)
        unit
    }],
    top_products [{         // En çok satan ürünler
        name, quantity, unit_price,
        total_revenue, market_share
    }],
    daily_sales [{          // Günlük satış detayı
        date, order_count,
        revenue, average_order
    }],
    notes (optional)
}
```

**Görsel Özellikler:**
- 📈 Özet kartlar (gradient)
- 📊 Performans metrikleri tablosu
- 🏆 En iyi ürünler sıralaması
- 📅 Günlük dağılım
- ✅ Progress bar'lar

---

#### 5. **inventory-report.html** - Stok Raporu
**Kullanım:** Envanter yönetimi, stok kontrol

**Özellikler:**
- 🚨 Kritik stok uyarıları (kırmızı arka plan)
- ⚠️ Düşük stok bildirimleri (sarı)
- ✅ Normal stok gösterimi (yeşil)
- 💰 Toplam stok değeri hesaplama

**Gerekli Model:**
```csharp
{
    company_name,
    products [{
        code, name,
        current_stock,
        min_stock,
        optimal_stock,
        unit_price
    }],
    critical_items,         // current_stock < min_stock olanlar
    total_value             // Toplam stok değeri
}
```

---

### 💵 Bordro Şablonları (Payroll)

#### 6. **monthly-payroll.html** - Aylık Bordro
**Kullanım:** Çalışan maaş bordrosu

**Gerekli Model:**
```csharp
{
    company_name,
    period_month, period_year,
    employee {
        name, id_number,
        department, position,
        hire_date
    },
    salary { gross },
    additional_earnings [{  // Ek kazançlar
        name, amount
    }],
    deductions [{           // Kesintiler
        name, amount
    }],
    total_earnings,
    total_deductions,
    net_salary
}
```

**Bölümler:**
- 👤 Çalışan bilgileri
- 💰 Gelirler (yeşil arka plan)
- ❌ Kesintiler (kırmızı arka plan)
- ✅ Net maaş (bold, büyük)

---

## 🚀 HIZLI BAŞLANGIÇ

### 1. TemplateManager ile Kullanım

```csharp
using CSB.Module.Report;
using CSB.Module.Report.Sample.Helpers;

// PDF oluştur
var pdfGen = ReportFactory.CreatePdf();

// Template yolu al
var templatePath = TemplateManager.Invoices.Modern;

// Model hazırla
var model = new { /* ... */ };

// Rapor oluştur
await pdfGen.GenerateFromFileAsync(templatePath, model);
```

### 2. Direkt Dosya Yolu ile

```csharp
await pdfGen.GenerateFromFileAsync(
    "Templates/Invoices/classic-invoice.html",
    model
);
```

### 3. Cache Kullanımı (Performans)

```csharp
// Template'i cache'den yükle
var template = await TemplateManager.LoadCachedAsync(
    "Invoices/modern-invoice.html"
);

// Inline kullan
await pdfGen.GenerateAsync(template, model);
```

---

## 💡 İPUÇLARI VE BEST PRACTICES

### 1. **Model Hazırlama**
```csharp
// ✅ DOĞRU: Nullable alanlar için null check
var model = new
{
    title = "Rapor",
    subtitle = report.Subtitle ?? "Alt başlık yok"
};

// ❌ YANLIŞ: Null değer gönderme
var model = new
{
    title = "Rapor",
    subtitle = report.Subtitle  // null olabilir!
};
```

### 2. **Sayfa Yönü Seçimi**
```csharp
// Landscape raporlar için
var options = new ReportOptions
{
    PageSize = PageSize.A4,
    Orientation = PageOrientation.Landscape
};
var pdfGen = ReportFactory.CreatePdf(options);
```

### 3. **Para Formatı**
```scriban
{{/* Scriban template'de */}}
{{ amount | math.format '0,0.00' }} TL
{{ amount | math.format '#,##0.00' }} USD
```

### 4. **Tarih Formatı**
```scriban
{{ date.now | date.to_string '%d.%m.%Y' }}           // 18.11.2025
{{ date.now | date.to_string '%d %B %Y' }}           // 18 Kasım 2025
{{ date.now | date.to_string '%d.%m.%Y %H:%M' }}     // 18.11.2025 14:30
```

### 5. **Koşullu Render**
```scriban
{{ if discount > 0 }}
    <p>İndirim: {{ discount }} TL</p>
{{ end }}
```

### 6. **Döngüler**
```scriban
{{ for product in products }}
    <tr>
        <td>{{ for.index + 1 }}</td>      {{/* Sıra numarası */}}
        <td>{{ product.name }}</td>
    </tr>
{{ end }}
```

---

## 🎨 ÖZELLEŞTİRME

### Renkleri Değiştirme
Template dosyasını açın ve `<style>` bölümünde renkleri değiştirin:

```css
/* Örnek: Modern invoice'ta */
.header-banner {
    background: linear-gradient(135deg, #YOUR_COLOR1 0%, #YOUR_COLOR2 100%);
}
```

### Yeni Alan Ekleme
1. Template'e HTML ekleyin:
```html
<p>Yeni Alan: {{ new_field }}</p>
```

2. Model'e alan ekleyin:
```csharp
var model = new
{
    new_field = "Değer",
    // ... diğer alanlar
};
```

---

## 📞 DESTEK

Template kullanımında sorun mu yaşıyorsunuz?

1. **Örnek Kod:** `Examples/TemplateExamples.cs` dosyasına bakın
2. **Scriban Dokümantasyonu:** https://github.com/scriban/scriban
3. **Model Doğrulama:** Template'te hangi alanları kullandığınızı kontrol edin

---

## ✅ CHECKLIST - Template Kullanmadan Önce

- [ ] Model tüm gerekli alanları içeriyor mu?
- [ ] Nullable alanlar için null check yapıldı mı?
- [ ] Tarih ve para formatları doğru mu?
- [ ] Output klasörü var mı?
- [ ] Template dosyası mevcut mu?

---

**🎉 Hazırsınız! Artık profesyonel raporlar oluşturabilirsiniz!**
