namespace AvaloniaSqliteCurve.Extensions;

public static class ColorExtension
{
    public static ScottPlot.Color ToScottPlotColor(this Avalonia.Media.Color color) =>
        new(color.R, color.G, color.B, color.A);
}