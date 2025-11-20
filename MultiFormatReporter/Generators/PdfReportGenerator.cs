using MultiFormatReporter.Interfaces;
using MultiFormatReporter.Interfaces.Core;
using MultiFormatReporter.Models;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using Scriban.Runtime;
using iTextPageSize = iText.Kernel.Geom.PageSize;

namespace MultiFormatReporter.Generators;

public class PdfReportGenerator : ReportGeneratorBase
{
    public PdfReportGenerator(
        ITemplateRenderer templateRenderer,
        ReportOptions? options = null)
        : base(templateRenderer, options)
    {
    }

    protected override async Task<byte[]> ConvertToDocumentAsync(string renderedHtml)
    {
        return await Task.Run(() =>
        {
            using var memoryStream = new MemoryStream();

            var writerProperties = new WriterProperties();
            using var pdfWriter = new PdfWriter(memoryStream, writerProperties);
            using var pdfDocument = new PdfDocument(pdfWriter);

            var pageSize = GetPageSize();
            pdfDocument.SetDefaultPageSize(pageSize);

            var converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(renderedHtml, pdfDocument, converterProperties);

            return memoryStream.ToArray();
        });
    }

    private iTextPageSize GetPageSize()
    {
        var pageSize = Options.PageSize switch
        {
            Models.PageSize.A4 => iTextPageSize.A4,
            Models.PageSize.A5 => iTextPageSize.A5,
            Models.PageSize.Letter => iTextPageSize.LETTER,
            Models.PageSize.Legal => iTextPageSize.LEGAL,
            _ => iTextPageSize.A4
        };

        if (Options.Orientation == PageOrientation.Landscape)
        {
            pageSize = pageSize.Rotate();
        }

        return pageSize;
    }

    public async Task<byte[]> GenerateAdvancedAsync(string template, object model, PdfMetadata? metadata = null)
    {
        var renderedHtml = await TemplateRenderer.RenderAsync(template, model);

        return await Task.Run(() =>
        {
            using var memoryStream = new MemoryStream();

            var writerProperties = new WriterProperties();
            using var pdfWriter = new PdfWriter(memoryStream, writerProperties);
            using var pdfDocument = new PdfDocument(pdfWriter);

            if (metadata != null)
            {
                var docInfo = pdfDocument.GetDocumentInfo();
                if (!string.IsNullOrEmpty(metadata.Title))
                    docInfo.SetTitle(metadata.Title);
                if (!string.IsNullOrEmpty(metadata.Author))
                    docInfo.SetAuthor(metadata.Author);
                if (!string.IsNullOrEmpty(metadata.Subject))
                    docInfo.SetSubject(metadata.Subject);
                if (!string.IsNullOrEmpty(metadata.Keywords))
                    docInfo.SetKeywords(metadata.Keywords);
            }

            var pageSize = GetPageSize();
            pdfDocument.SetDefaultPageSize(pageSize);

            var converterProperties = new ConverterProperties();
            HtmlConverter.ConvertToPdf(renderedHtml, pdfDocument, converterProperties);

            return memoryStream.ToArray();
        });
    }
}

public class PdfMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
}
