using MultiFormatReporter.Generators;
using MultiFormatReporter.Interfaces;
using MultiFormatReporter.Models;

namespace MultiFormatReporter;

public class ReportFactory: IReportGeneratorFactory
{
    private readonly ITemplateRenderer _templateRenderer;

    public ReportFactory(ITemplateRenderer templateRenderer)
    {
        _templateRenderer = templateRenderer ?? throw new ArgumentNullException(nameof(templateRenderer));
    }

    public ITemplateBasedReportGenerator CreatePdf(ReportOptions? options = null)
    {
        return new PdfReportGenerator(_templateRenderer, options);
    }

    public ITemplateBasedReportGenerator CreateWord(ReportOptions? options = null)
    {
        return new WordReportGenerator(_templateRenderer, options);
    }

    public IDataBasedReportGenerator CreateExcel(ReportOptions? options = null)
    {
        return new ExcelReportGenerator(options);
    }
}
