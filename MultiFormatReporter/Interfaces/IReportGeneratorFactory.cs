using MultiFormatReporter.Generators;
using MultiFormatReporter.Models;

namespace MultiFormatReporter.Interfaces;

public interface IReportGeneratorFactory
{
    ITemplateBasedReportGenerator CreatePdf(ReportOptions? options = null);
    ITemplateBasedReportGenerator CreateWord(ReportOptions? options = null);
    IDataBasedReportGenerator CreateExcel(ReportOptions? options = null);
}
