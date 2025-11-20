using MultiFormatReporter.Generators;
using MultiFormatReporter.Interfaces;
using MultiFormatReporter.Models;
using MultiFormatReporter.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MultiFormatReporter.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReportingServices(this IServiceCollection services)
    {
        services.AddSingleton<ITemplateRenderer, ScribanTemplateRenderer>();
        services.AddSingleton<IReportGeneratorFactory, ReportFactory>();

        return services;
    }

    public static IServiceCollection AddPdfReportGenerator(
        this IServiceCollection services,
        Action<ReportOptions>? configureOptions = null)
    {
        services.AddReportingServices();

        services.AddTransient<ITemplateBasedReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new PdfReportGenerator(templateRenderer, options);
        });

        services.AddTransient<PdfReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new PdfReportGenerator(templateRenderer, options);
        });

        return services;
    }

    public static IServiceCollection AddWordReportGenerator(
        this IServiceCollection services,
        Action<ReportOptions>? configureOptions = null)
    {
        services.AddReportingServices();

        services.AddTransient<ITemplateBasedReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new WordReportGenerator(templateRenderer, options);
        });

        services.AddTransient<WordReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new WordReportGenerator(templateRenderer, options);
        });

        return services;
    }

    public static IServiceCollection AddExcelReportGenerator(
        this IServiceCollection services,
        Action<ReportOptions>? configureOptions = null)
    {
        services.AddReportingServices();

        services.AddTransient<IDataBasedReportGenerator>(sp =>
        {
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new ExcelReportGenerator(options);
        });

        services.AddTransient<ExcelReportGenerator>(sp =>
        {
            var options = new ReportOptions();
            configureOptions?.Invoke(options);
            return new ExcelReportGenerator(options);
        });

        return services;
    }

    public static IServiceCollection AddReportGenerators(
        this IServiceCollection services,
        Action<ReportOptions>? configurePdfOptions = null,
        Action<ReportOptions>? configureWordOptions = null,
        Action<ReportOptions>? configureExcelOptions = null)
    {
        services.AddReportingServices();

        services.AddTransient<PdfReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configurePdfOptions?.Invoke(options);
            return new PdfReportGenerator(templateRenderer, options);
        });

        services.AddTransient<WordReportGenerator>(sp =>
        {
            var templateRenderer = sp.GetRequiredService<ITemplateRenderer>();
            var options = new ReportOptions();
            configureWordOptions?.Invoke(options);
            return new WordReportGenerator(templateRenderer, options);
        });

        services.AddTransient<ExcelReportGenerator>(sp =>
        {
            var options = new ReportOptions();
            configureExcelOptions?.Invoke(options);
            return new ExcelReportGenerator(options);
        });

        return services;
    }
}
