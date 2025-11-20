namespace MultiFormatReporter.Interfaces;

public interface ITemplateBasedReportGenerator
{
    Task<byte[]> GenerateAsync(string template, object model);
    Task<byte[]> GenerateFromFileAsync(string templatePath, object model);
    Task SaveAsync(string template, object model, string outputPath);
}
