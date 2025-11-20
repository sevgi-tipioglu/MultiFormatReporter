namespace MultiFormatReporter.Models;

public class ReportOptions
{
    public PageSize PageSize { get; set; } = PageSize.A4;

    public PageOrientation Orientation { get; set; } = PageOrientation.Portrait;

    public PageMargins Margins { get; set; } = new PageMargins
    {
        Top = 25,
        Right = 25,
        Bottom = 25,
        Left = 25
    };
}

public enum PageSize
{
    A4,
    A5,
    Letter,
    Legal
}

public enum PageOrientation
{
    Portrait,
    Landscape
}

public class PageMargins
{
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }
    public float Left { get; set; }
}
