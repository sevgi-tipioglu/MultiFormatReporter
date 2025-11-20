using MultiFormatReporter.Interfaces;
using MultiFormatReporter.Interfaces.Core;
using MultiFormatReporter.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WordPageSize = DocumentFormat.OpenXml.Wordprocessing.PageSize;

namespace MultiFormatReporter.Generators;

public class WordReportGenerator : ReportGeneratorBase
{
    public WordReportGenerator(
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

            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                var altChunkId = "AltChunkId1";
                var chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);

                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(renderedHtml)))
                {
                    chunk.FeedData(stream);
                }

                var altChunk = new AltChunk { Id = altChunkId };
                body.Append(altChunk);

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return memoryStream.ToArray();
        });
    }

    public async Task<byte[]> GenerateAdvancedAsync(string template, object model, WordMetadata? metadata = null)
    {
        var renderedHtml = await TemplateRenderer.RenderAsync(template, model);

        return await Task.Run(() =>
        {
            using var memoryStream = new MemoryStream();

            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = new Body();

                if (metadata != null)
                {
                    var properties = wordDocument.PackageProperties;
                    properties.Title = metadata.Title ?? string.Empty;
                    properties.Creator = metadata.Author ?? string.Empty;
                    properties.Subject = metadata.Subject ?? string.Empty;
                    properties.Keywords = metadata.Keywords ?? string.Empty;
                }

                var sectionProperties = CreateSectionProperties();
                body.Append(sectionProperties);

                var altChunkId = "AltChunkId1";
                var chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.Html, altChunkId);

                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(renderedHtml)))
                {
                    chunk.FeedData(stream);
                }

                var altChunk = new AltChunk { Id = altChunkId };
                body.InsertBefore(altChunk, sectionProperties);

                mainPart.Document.Append(body);
                mainPart.Document.Save();
            }

            return memoryStream.ToArray();
        });
    }

    private SectionProperties CreateSectionProperties()
    {
        var sectionProps = new SectionProperties();

        var pageSize = new WordPageSize
        {
            Width = GetPageWidth(),
            Height = GetPageHeight()
        };

        var pageMargin = new PageMargin
        {
            Top = (int)(Options.Margins.Top * 56.7),
            Right = (uint)(Options.Margins.Right * 56.7),
            Bottom = (int)(Options.Margins.Bottom * 56.7),
            Left = (uint)(Options.Margins.Left * 56.7)
        };

        sectionProps.Append(pageSize);
        sectionProps.Append(pageMargin);

        return sectionProps;
    }

    private uint GetPageWidth()
    {
        return Options.PageSize switch
        {
            Models.PageSize.A4 => Options.Orientation == PageOrientation.Portrait ? 11906u : 16838u,
            Models.PageSize.A5 => Options.Orientation == PageOrientation.Portrait ? 8391u : 11906u,
            Models.PageSize.Letter => Options.Orientation == PageOrientation.Portrait ? 12240u : 15840u,
            Models.PageSize.Legal => Options.Orientation == PageOrientation.Portrait ? 12240u : 20160u,
            _ => 11906u
        };
    }

    private uint GetPageHeight()
    {
        return Options.PageSize switch
        {
            Models.PageSize.A4 => Options.Orientation == PageOrientation.Portrait ? 16838u : 11906u,
            Models.PageSize.A5 => Options.Orientation == PageOrientation.Portrait ? 11906u : 8391u,
            Models.PageSize.Letter => Options.Orientation == PageOrientation.Portrait ? 15840u : 12240u,
            Models.PageSize.Legal => Options.Orientation == PageOrientation.Portrait ? 20160u : 12240u,
            _ => 16838u
        };
    }
}

public class WordMetadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
}
