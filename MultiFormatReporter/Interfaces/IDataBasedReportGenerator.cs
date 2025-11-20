namespace MultiFormatReporter.Interfaces;

public interface IDataBasedReportGenerator
{
    Task<byte[]> GenerateTableAsync<T>(IEnumerable<T> data, string sheetName = "Sayfa1") where T : class;
    Task<byte[]> GenerateFromDynamicAsync(IEnumerable<object> data, string sheetName = "Sayfa1");
    Task SaveAsync<T>(IEnumerable<T> data, string outputPath, string sheetName = "Sayfa1") where T : class;
}
