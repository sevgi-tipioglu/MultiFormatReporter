using ClosedXML.Excel;
using MultiFormatReporter.Interfaces;
using MultiFormatReporter.Models;
using System.Reflection;

namespace MultiFormatReporter.Generators;

public class ExcelReportGenerator : IDataBasedReportGenerator
{
    protected readonly ReportOptions Options;

    public ExcelReportGenerator(ReportOptions? options = null)
    {
        Options = options ?? new ReportOptions();
    }

    public async Task<byte[]> GenerateTableAsync<T>(IEnumerable<T> data, string sheetName = "Sayfa1") where T : class
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            if (data == null || !data.Any())
            {
                return WorkbookToBytes(workbook);
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    var value = properties[col].GetValue(item);
                    worksheet.Cell(row, col + 1).Value = value?.ToString() ?? string.Empty;
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            return WorkbookToBytes(workbook);
        });
    }

    [Obsolete("Bu metod abstraction leak içerir (ClosedXML'e doğrudan bağımlılık). GenerateTableAsync veya GenerateFromDynamicAsync metodlarını kullanın.", false)]
    public async Task<byte[]> GenerateCustomAsync(Action<IXLWorkbook> configureWorkbook)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            configureWorkbook(workbook);
            return WorkbookToBytes(workbook);
        });
    }

    public async Task<byte[]> GenerateFromDynamicAsync(IEnumerable<object> data, string sheetName = "Sayfa1")
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            if (data == null || !data.Any())
            {
                return WorkbookToBytes(workbook);
            }

            var firstItem = data.First();
            var properties = firstItem.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.LightBlue;
            }

            int row = 2;
            foreach (var item in data)
            {
                var itemProperties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                for (int col = 0; col < itemProperties.Length; col++)
                {
                    var value = itemProperties[col].GetValue(item);
                    if (value != null)
                    {
                        if (value is int || value is long || value is decimal || value is double || value is float)
                        {
                            worksheet.Cell(row, col + 1).Value = Convert.ToDouble(value);
                        }
                        else if (value is DateTime dateTime)
                        {
                            worksheet.Cell(row, col + 1).Value = dateTime;
                            worksheet.Cell(row, col + 1).Style.DateFormat.Format = "dd.MM.yyyy";
                        }
                        else
                        {
                            worksheet.Cell(row, col + 1).Value = value.ToString();
                        }
                    }
                }
                row++;
            }

            worksheet.Columns().AdjustToContents();

            return WorkbookToBytes(workbook);
        });
    }

    private byte[] WorkbookToBytes(XLWorkbook workbook)
    {
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task SaveAsync<T>(IEnumerable<T> data, string outputPath, string sheetName = "Sayfa1") where T : class
    {
        var bytes = await GenerateTableAsync(data, sheetName);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, bytes);
    }

    public async Task SaveFromDynamicAsync(IEnumerable<object> data, string outputPath, string sheetName = "Sayfa1")
    {
        var bytes = await GenerateFromDynamicAsync(data, sheetName);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, bytes);
    }

    [Obsolete("Bu metod abstraction leak içerir (ClosedXML'e doğrudan bağımlılık). SaveAsync metodunu kullanın.", false)]
    public async Task SaveCustomAsync(Action<IXLWorkbook> configureWorkbook, string outputPath)
    {
        var bytes = await GenerateCustomAsync(configureWorkbook);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, bytes);
    }
}
