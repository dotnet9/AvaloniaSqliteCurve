using CfPlot.Avalonia.Primitives;

// ReSharper disable once CheckNamespace
namespace CfPlot.Avalonia;

public class LineStyle
{
    public float Width { get; set; } = 0;
    public Color Color { get; set; } = Colors.Black;
    public LinePattern Pattern { get; set; } = LinePattern.Solid;
    public static LineStyle None => new() { Width = 0 };

    public LineStyle()
    {
    }

    public LineStyle(float width)
    {
        Width = width;
    }

    public LineStyle(float width, Color color)
    {
        Width = width;
        Color = color;
    }

    public LineStyle(float width, Color color, LinePattern pattern)
    {
        Width = width;
        Color = color;
        Pattern = pattern;
    }
}