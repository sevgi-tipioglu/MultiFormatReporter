namespace MultiFormatReporter.Sample.Helpers;

public static class TemplateManager
{
    private static readonly string TemplateBasePath = Path.Combine("Templates");

    public static class Invoices
    {
        public static string Classic => GetPath("Invoices", "classic-invoice.html");
        public static string Modern => GetPath("Invoices", "modern-invoice.html");
        public static string Minimal => GetPath("Invoices", "minimal-invoice.html");
    }

    public static class Reports
    {
        public static string Sales => GetPath("Reports", "sales-report.html");
        public static string Inventory => GetPath("Reports", "inventory-report.html");
        public static string Simple => GetPath("Reports", "simple-report.html");
        public static string Product => GetPath("Reports", "product-report.html");
        public static string Employee => GetPath("Reports", "employee-report.html");
    }

    public static class Payroll
    {
        public static string Monthly => GetPath("Payroll", "monthly-payroll.html");
    }

    private static string GetPath(string category, string fileName)
    {
        return Path.Combine(TemplateBasePath, category, fileName);
    }
}
