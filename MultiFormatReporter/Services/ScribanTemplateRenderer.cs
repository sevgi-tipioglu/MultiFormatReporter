using MultiFormatReporter.Interfaces;
using Scriban;

namespace MultiFormatReporter.Services;

public class ScribanTemplateRenderer : ITemplateRenderer
{
    public async Task<string> RenderAsync(string template, object model)
    {
        if (string.IsNullOrWhiteSpace(template))
        {
            throw new ArgumentException("Template boş olamaz.", nameof(template));
        }

        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "Model null olamaz.");
        }

        try
        {
            var scribanTemplate = Template.Parse(template);

            if (scribanTemplate.HasErrors)
            {
                var errors = string.Join(", ", scribanTemplate.Messages);
                throw new InvalidOperationException($"Template parse hatası: {errors}");
            }

            return await scribanTemplate.RenderAsync(model);
        }
        catch (Exception ex) when (ex is not ArgumentException && ex is not ArgumentNullException)
        {
            throw new InvalidOperationException($"Template render hatası: {ex.Message}", ex);
        }
    }
}
