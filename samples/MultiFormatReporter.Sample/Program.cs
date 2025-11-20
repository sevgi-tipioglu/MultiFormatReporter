using MultiFormatReporter;
using MultiFormatReporter.Generators;
using MultiFormatReporter.Models;
using MultiFormatReporter.Sample.Helpers;
using MultiFormatReporter.Services;

namespace MultiFormatReporter.Sample;

class Program
{
    private static string OutputDirectory = string.Empty;

    static async Task Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   MultiFormatReporter - Scriban Template Örnekleri         ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        var projectDir = Directory.GetCurrentDirectory();
        while (!File.Exists(Path.Combine(projectDir, "MultiFormatReporter.Sample.csproj")))
        {
            var parent = Directory.GetParent(projectDir);
            if (parent == null) break;
            projectDir = parent.FullName;
        }
        OutputDirectory = Path.Combine(projectDir, "Output");
        Directory.CreateDirectory(OutputDirectory);

        Console.WriteLine("┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  BÖLÜM 1: Temel Örnekler (Inline Template)             │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘\n");

        await CreateSimplePdfReport();
        await CreateAdvancedPdfReport();
        await CreateWordReport();

        Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  BÖLÜM 2: Fatura Şablonları (Template Dosyaları)       │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘\n");

        await CreateClassicInvoice();
        await CreateModernInvoice();
        await CreateMinimalInvoice();

        Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  BÖLÜM 3: Rapor Şablonları                              │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘\n");

        await CreateSalesReport();
        await CreateInventoryReport();

        Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  BÖLÜM 4: Bordro Şablonu                                │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘\n");

        await CreatePayrollReport();

        Console.WriteLine("\n┌─────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  BÖLÜM 5: Excel Raporları                               │");
        Console.WriteLine("└─────────────────────────────────────────────────────────┘\n");

        await CreateSimpleExcelReport();
        await CreateProductExcelReport();
        await CreateCustomExcelReport();

        Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  ✓ Tüm raporlar başarıyla oluşturuldu!                   ║");
        Console.WriteLine($"║  📁 Klasör: {OutputDirectory}");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝");
    }

    static async Task CreateSimplePdfReport()
    {
        Console.WriteLine("1. Basit PDF Rapor oluşturuluyor...");

        var pdfGenerator = new PdfReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            title = "Basit PDF Raporu",
            name = "Kullanıcı",
            items = new[] { "Öğe 1", "Öğe 2", "Öğe 3", "Öğe 4" }
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "01-basit-rapor.pdf"),
            await pdfGenerator.GenerateFromFileAsync(TemplateManager.Reports.Simple, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "01-basit-rapor.pdf")}\n");
    }

    static async Task CreateAdvancedPdfReport()
    {
        Console.WriteLine("2. Gelişmiş PDF Rapor oluşturuluyor...");

        var options = new ReportOptions
        {
            PageSize = PageSize.A4,
            Orientation = PageOrientation.Portrait,
            Margins = new PageMargins { Top = 30, Right = 30, Bottom = 30, Left = 30 }
        };

        var pdfGenerator = new PdfReportGenerator(new ScribanTemplateRenderer(), options);

        var model = new
        {
            title = "Ürün Raporu",
            report_date = DateTime.Now.ToString("dd.MM.yyyy"),
            prepared_by = "Test Sistem",
            products = new[]
            {
                new { name = "Laptop", category = "Elektronik", price = 15000 },
                new { name = "Mouse", category = "Aksesuar", price = 150 },
                new { name = "Klavye", category = "Aksesuar", price = 500 },
                new { name = "Monitor", category = "Elektronik", price = 3000 }
            }
        };

        var metadata = new PdfMetadata
        {
            Title = "Ürün Raporu",
            Author = "Test",
            Subject = "Ürün Listesi",
            Keywords = "ürün, rapor, liste"
        };

        var template = await File.ReadAllTextAsync(TemplateManager.Reports.Product);
        var pdfData = await pdfGenerator.GenerateAdvancedAsync(template, model, metadata);
        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "02-urun-raporu.pdf"), pdfData);

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "02-urun-raporu.pdf")}\n");
    }

    static async Task CreateWordReport()
    {
        Console.WriteLine("3. Word Rapor oluşturuluyor...");

        var wordGenerator = new WordReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            title = "Çalışan Raporu",
            report_date = DateTime.Now.ToString("dd.MM.yyyy"),
            employees = new[]
            {
                new { name = "Ahmet Yılmaz", department = "IT", salary = 25000 },
                new { name = "Ayşe Demir", department = "İK", salary = 22000 },
                new { name = "Mehmet Kaya", department = "Muhasebe", salary = 20000 }
            }
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "03-calisan-raporu.docx"),
            await wordGenerator.GenerateFromFileAsync(TemplateManager.Reports.Employee, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "03-calisan-raporu.docx")}\n");
    }

    static async Task CreateClassicInvoice()
    {
        Console.WriteLine("4. Klasik Fatura (Template) oluşturuluyor...");

        var pdfGen = new PdfReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            invoice_number = "INV-2025-001",
            invoice_date = DateTime.Now.ToString("dd.MM.yyyy"),
            due_date = DateTime.Now.AddDays(30).ToString("dd.MM.yyyy"),
            payment_status = "Ödenmedi",
            currency = "TL",

            seller = new
            {
                company_name = "ABC Teknoloji A.Ş.",
                address = "Atatürk Cad. No:123",
                city = "İstanbul",
                country = "Türkiye",
                tax_number = "1234567890",
                phone = "+90 212 555 1234",
                email = "info@abcteknoloji.com",
                website = "www.abcteknoloji.com"
            },

            customer = new
            {
                name = "XYZ Şirketi Ltd. Şti.",
                address = "Cumhuriyet Mah. Yeni Sok. No:45",
                city = "Ankara",
                postal_code = "06100",
                country = "Türkiye",
                tax_number = "9876543210",
                phone = "+90 312 555 4321",
                email = "iletisim@xyzfirma.com"
            },

            line_items = new[]
            {
                new { product = "Yazılım Lisansı (Yıllık)", description = "Enterprise sürüm, 100 kullanıcı", quantity = 1, unit_price = 50000.00 },
                new { product = "Teknik Destek Paketi", description = "7/24 premium destek hizmeti", quantity = 12, unit_price = 2500.00 },
                new { product = "Eğitim Hizmeti", description = "3 günlük yerinde eğitim", quantity = 1, unit_price = 15000.00 }
            },

            subtotal = 95000.00,
            discount = 4750.00,
            discount_rate = 5,
            vat_rate = 20,
            vat_amount = 18050.00,
            total = 108300.00,

            notes = "Ödeme, fatura tarihinden itibaren 30 gün içerisinde yapılmalıdır.",
            payment_terms = "Havale/EFT ile ödeme yapılabilir."
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "04-classic-invoice.pdf"),
            await pdfGen.GenerateFromFileAsync(TemplateManager.Invoices.Classic, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "04-classic-invoice.pdf")}\n");
    }

    static async Task CreateModernInvoice()
    {
        Console.WriteLine("5. Modern Fatura (Template) oluşturuluyor...");

        var pdfGen = new PdfReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            invoice_number = "INV-2025-002",
            invoice_date = DateTime.Now.ToString("dd.MM.yyyy"),
            payment_status = "Ödendi",
            currency = "TL",

            seller = new
            {
                company_name = "TechStart Yazılım",
                address = "Teknokent Binası Kat:5",
                city = "İzmir",
                postal_code = "35100",
                country = "Türkiye",
                tax_number = "5551234567",
                phone = "+90 232 555 7890",
                email = "contact@techstart.com",
                website = "www.techstart.com"
            },

            customer = new
            {
                name = "Ahmet Yılmaz",
                address = "Karşıyaka Mah. 123. Sok. No:7",
                city = "İzmir",
                postal_code = "35100",
                country = "Türkiye",
                tax_number = (string?)null,
                phone = "+90 532 111 2233",
                email = "ahmet.yilmaz@example.com"
            },

            line_items = new[]
            {
                new { product = "Web Tasarım Hizmeti", description = "Kurumsal web sitesi", quantity = 1, unit_price = 25000.00 },
                new { product = "SEO Optimizasyonu", description = "6 aylık SEO", quantity = 6, unit_price = 2000.00 }
            },

            subtotal = 37000.00,
            discount = 0.0,
            discount_rate = 0,
            vat_rate = 20,
            vat_amount = 7400.00,
            total = 44400.00,

            notes = "Proje tamamlandı ve müşteri tarafından onaylandı. Teşekkür ederiz!"
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "05-modern-invoice.pdf"),
            await pdfGen.GenerateFromFileAsync(TemplateManager.Invoices.Modern, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "05-modern-invoice.pdf")}\n");
    }

    static async Task CreateMinimalInvoice()
    {
        Console.WriteLine("6. Minimal Fatura (Template) oluşturuluyor...");

        var pdfGen = new PdfReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            invoice_number = "INV-2025-003",
            invoice_date = DateTime.Now.ToString("dd.MM.yyyy"),
            due_date = DateTime.Now.AddDays(15).ToString("dd.MM.yyyy"),
            payment_status = "Ödenmedi",
            currency = "TL",

            seller = new
            {
                company_name = "Design Studio",
                address = "Nişantaşı, İstanbul",
                city = "İstanbul",
                phone = "+90 212 333 4444",
                email = "hello@designstudio.com"
            },

            customer = new
            {
                name = "Mehmet Demir",
                address = "Kadıköy, İstanbul",
                city = "İstanbul",
                postal_code = "34710",
                tax_number = (string?)null
            },

            line_items = new[]
            {
                new { product = "Logo Tasarımı", description = "Kurumsal logo paketi", quantity = 1, unit_price = 5000.00 },
                new { product = "Kartvizit Tasarımı", description = (string?)null, quantity = 1, unit_price = 1000.00 }
            },

            subtotal = 6000.00,
            discount = 0.0,
            discount_rate = 0,
            vat_rate = 20,
            vat_amount = 1200.00,
            total = 7200.00,

            notes = (string?)null,
            payment_terms = "Payment due within 15 days."
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "06-minimal-invoice.pdf"),
            await pdfGen.GenerateFromFileAsync(TemplateManager.Invoices.Minimal, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "06-minimal-invoice.pdf")}\n");
    }

    static async Task CreateSalesReport()
    {
        Console.WriteLine("7. Satış Raporu (Template) oluşturuluyor...");

        var pdfGen = new PdfReportGenerator(new ScribanTemplateRenderer(), new ReportOptions
        {
            PageSize = PageSize.A4,
            Orientation = PageOrientation.Landscape
        });

        var model = new
        {
            report_title = "Aylık Satış Performans Raporu",
            company_name = "ABC Teknoloji A.Ş.",
            period_start = "01.01.2025",
            period_end = "31.01.2025",
            currency = "TL",

            summary = new
            {
                total_sales = 1250,
                total_revenue = 3850000.00,
                average_order = 3080.00,
                total_customers = 487
            },

            performance_metrics = new[]
            {
                new { name = "Toplam Gelir", current = 3850000.00, previous = 3200000.00, change = 20.31, progress = 85, unit = "TL" },
                new { name = "Ortalama Sipariş", current = 3080.00, previous = 2950.00, change = 4.41, progress = 75, unit = "TL" },
                new { name = "Müşteri Sayısı", current = 487.00, previous = 445.00, change = 9.44, progress = 90, unit = "" }
            },

            top_products = new[]
            {
                new { name = "Premium Yazılım Lisansı", quantity = 245, unit_price = 5000.00, total_revenue = 1225000.00, market_share = 31.82 },
                new { name = "Standart Lisans", quantity = 580, unit_price = 2000.00, total_revenue = 1160000.00, market_share = 30.13 },
                new { name = "Destek Paketi", quantity = 350, unit_price = 1500.00, total_revenue = 525000.00, market_share = 13.64 }
            },

            daily_sales = new[]
            {
                new { date = "01.01.2025", order_count = 42, revenue = 129600.00, average_order = 3085.71 },
                new { date = "02.01.2025", order_count = 38, revenue = 118240.00, average_order = 3111.58 },
                new { date = "03.01.2025", order_count = 45, revenue = 140850.00, average_order = 3130.00 }
            },

            notes = "Ocak ayı satışlarında önceki aya göre %20.31'lik artış gözlemlendi."
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "07-sales-report.pdf"),
            await pdfGen.GenerateFromFileAsync(TemplateManager.Reports.Sales, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "07-sales-report.pdf")}\n");
    }

    static async Task CreateInventoryReport()
    {
        Console.WriteLine("8. Stok Raporu (Template) oluşturuluyor...");

        var wordGen = new WordReportGenerator(new ScribanTemplateRenderer());

        var products = new[]
        {
            new { code = "PRD-001", name = "Laptop Dell XPS 15", current_stock = 5, min_stock = 10, optimal_stock = 25, unit_price = 35000.00 },
            new { code = "PRD-002", name = "iPhone 15 Pro", current_stock = 3, min_stock = 15, optimal_stock = 30, unit_price = 45000.00 },
            new { code = "PRD-003", name = "Samsung Galaxy S24", current_stock = 25, min_stock = 15, optimal_stock = 30, unit_price = 30000.00 },
            new { code = "PRD-004", name = "MacBook Pro M3", current_stock = 12, min_stock = 8, optimal_stock = 20, unit_price = 65000.00 }
        };

        var model = new
        {
            company_name = "Teknoloji Mağazası",
            products = products,
            critical_items = products.Where(p => p.current_stock < p.min_stock).ToArray(),
            total_value = products.Sum(p => p.current_stock * p.unit_price)
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "08-inventory-report.docx"),
            await wordGen.GenerateFromFileAsync(TemplateManager.Reports.Inventory, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "08-inventory-report.docx")}\n");
    }

    static async Task CreatePayrollReport()
    {
        Console.WriteLine("9. Bordro Raporu (Template) oluşturuluyor...");

        var pdfGen = new PdfReportGenerator(new ScribanTemplateRenderer());

        var model = new
        {
            company_name = "ABC Teknoloji A.Ş.",
            period_month = "Ocak",
            period_year = 2025,

            employee = new
            {
                name = "Mehmet Demir",
                id_number = "12345678901",
                department = "Yazılım Geliştirme",
                position = "Senior Developer",
                hire_date = "15.03.2020"
            },

            salary = new { gross = 65000.00 },

            additional_earnings = new[]
            {
                new { name = "Yemek Kartı", amount = 3000.00 },
                new { name = "Performans Primi", amount = 10000.00 },
                new { name = "Ulaşım Desteği", amount = 2000.00 }
            },

            deductions = new[]
            {
                new { name = "SGK Primi (%14)", amount = 9100.00 },
                new { name = "İşsizlik Sigortası (%1)", amount = 650.00 },
                new { name = "Gelir Vergisi (%27)", amount = 17550.00 }
            },

            total_earnings = 80000.00,
            total_deductions = 27300.00,
            net_salary = 52700.00
        };

        await File.WriteAllBytesAsync(Path.Combine(OutputDirectory, "09-payroll.pdf"),
            await pdfGen.GenerateFromFileAsync(TemplateManager.Payroll.Monthly, model));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "09-payroll.pdf")}\n");
    }

    static async Task CreateSimpleExcelReport()
    {
        Console.WriteLine("10. Basit Excel Raporu oluşturuluyor...");

        var excelGen = new ExcelReportGenerator();

        var data = new[]
        {
            new { Urun = "Laptop", Kategori = "Elektronik", Fiyat = 15000, Stok = 45 },
            new { Urun = "Mouse", Kategori = "Aksesuar", Fiyat = 150, Stok = 230 },
            new { Urun = "Klavye", Kategori = "Aksesuar", Fiyat = 500, Stok = 120 },
            new { Urun = "Monitor", Kategori = "Elektronik", Fiyat = 3000, Stok = 67 }
        };

        await excelGen.SaveFromDynamicAsync(data, Path.Combine(OutputDirectory, "10-basit-excel.xlsx"), "Ürün Listesi");

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "10-basit-excel.xlsx")}\n");
    }

    static async Task CreateProductExcelReport()
    {
        Console.WriteLine("11. Ürün Excel Raporu oluşturuluyor...");

        var excelGen = new ExcelReportGenerator();

        var products = new[]
        {
            new { Kod = "P001", UrunAdi = "Laptop Dell XPS", Kategori = "Bilgisayar", BirimFiyat = 25000.00, Stok = 15, ToplamDeger = 375000.00 },
            new { Kod = "P002", UrunAdi = "iPhone 15 Pro", Kategori = "Telefon", BirimFiyat = 55000.00, Stok = 8, ToplamDeger = 440000.00 },
            new { Kod = "P003", UrunAdi = "Samsung Monitor 27\"", Kategori = "Monitör", BirimFiyat = 8500.00, Stok = 22, ToplamDeger = 187000.00 },
            new { Kod = "P004", UrunAdi = "Logitech MX Master 3", Kategori = "Aksesuar", BirimFiyat = 2500.00, Stok = 45, ToplamDeger = 112500.00 },
            new { Kod = "P005", UrunAdi = "iPad Air", Kategori = "Tablet", BirimFiyat = 28000.00, Stok = 12, ToplamDeger = 336000.00 }
        };

        await excelGen.SaveFromDynamicAsync(products, Path.Combine(OutputDirectory, "11-urun-excel.xlsx"), "Ürün Envanteri");

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "11-urun-excel.xlsx")}\n");
    }

    static async Task CreateCustomExcelReport()
    {
        Console.WriteLine("12. Özel Excel Raporu oluşturuluyor...");

        var excelGen = new ExcelReportGenerator();

        await excelGen.SaveCustomAsync(workbook =>
        {
            var summary = workbook.Worksheets.Add("Özet");
            summary.Cell("A1").Value = "AYLIK SATIŞ RAPORU";
            summary.Cell("A1").Style.Font.Bold = true;
            summary.Cell("A1").Style.Font.FontSize = 16;
            summary.Range("A1:D1").Merge();

            summary.Cell("A3").Value = "Toplam Satış:";
            summary.Cell("B3").Value = 1450000;
            summary.Cell("B3").Style.NumberFormat.Format = "#,##0.00 ₺";

            summary.Cell("A4").Value = "Toplam Sipariş:";
            summary.Cell("B4").Value = 245;

            summary.Cell("A5").Value = "Ortalama Sepet:";
            summary.Cell("B5").Value = 5918.37;
            summary.Cell("B5").Style.NumberFormat.Format = "#,##0.00 ₺";

            var details = workbook.Worksheets.Add("Satış Detayı");

            details.Cell("A1").Value = "Tarih";
            details.Cell("B1").Value = "Ürün";
            details.Cell("C1").Value = "Adet";
            details.Cell("D1").Value = "Tutar";

            var headerRange = details.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(79, 129, 189);
            headerRange.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

            var salesData = new[]
            {
                new { Tarih = DateTime.Now.AddDays(-5), Urun = "Laptop", Adet = 3, Tutar = 75000.00 },
                new { Tarih = DateTime.Now.AddDays(-4), Urun = "Mouse", Adet = 15, Tutar = 2250.00 },
                new { Tarih = DateTime.Now.AddDays(-3), Urun = "Klavye", Adet = 12, Tutar = 6000.00 },
                new { Tarih = DateTime.Now.AddDays(-2), Urun = "Monitor", Adet = 8, Tutar = 24000.00 },
                new { Tarih = DateTime.Now.AddDays(-1), Urun = "Tablet", Adet = 5, Tutar = 35000.00 }
            };

            int row = 2;
            foreach (var sale in salesData)
            {
                details.Cell(row, 1).Value = sale.Tarih;
                details.Cell(row, 1).Style.DateFormat.Format = "dd.MM.yyyy";
                details.Cell(row, 2).Value = sale.Urun;
                details.Cell(row, 3).Value = sale.Adet;
                details.Cell(row, 4).Value = sale.Tutar;
                details.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00 ₺";
                row++;
            }

            details.Columns().AdjustToContents();
            summary.Columns().AdjustToContents();

        }, Path.Combine(OutputDirectory, "12-ozel-excel.xlsx"));

        Console.WriteLine($"   ✓ Oluşturuldu: {Path.Combine(OutputDirectory, "12-ozel-excel.xlsx")}\n");
    }
    

    }
