using MultiFormatReporter.Models;

namespace MultiFormatReporter.Interfaces.Core;

public abstract class ReportGeneratorBase : ITemplateBasedReportGenerator
{
    protected readonly ReportOptions Options;
    protected readonly ITemplateRenderer TemplateRenderer;

    protected ReportGeneratorBase(
        ITemplateRenderer templateRenderer,
        ReportOptions? options = null)
    {
        TemplateRenderer = templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));
        Options = options ?? new ReportOptions();
    }

    protected abstract Task<byte[]> ConvertToDocumentAsync(string renderedHtml);

    public async Task<byte[]> GenerateAsync(string template, object model)
    {
        var renderedHtml = await TemplateRenderer.RenderAsync(template, model);
        return await ConvertToDocumentAsync(renderedHtml);
    }

    public async Task<byte[]> GenerateFromFileAsync(string templatePath, object model)
    {
        var template = await File.ReadAllTextAsync(templatePath);
        return await GenerateAsync(template, model);
    }

    public async Task SaveAsync(string template, object model, string outputPath)
    {
        var data = await GenerateAsync(template, model);

        var directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await File.WriteAllBytesAsync(outputPath, data);
    }
}
