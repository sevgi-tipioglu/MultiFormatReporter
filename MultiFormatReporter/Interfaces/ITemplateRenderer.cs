namespace MultiFormatReporter.Interfaces;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(string template, object model);
}
