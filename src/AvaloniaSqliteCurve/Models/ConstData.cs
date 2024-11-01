namespace AvaloniaSqliteCurve.Models;

public class ConstData
{
    public const int LineCount = 16;
    public const int DisplayMaxPointsCount = 2000;
    public const float DefaultLineWidth = 1f;

    public const double MinBottom = -300.0;
    public const double MaxTop = 300.0;

    public const int AddDataInterval = 50;
    public const int UpdateDataInterval = 10;


    public static Avalonia.Media.Color Fill = Avalonia.Media.Colors.White;
    public static Avalonia.Media.Color Stroke = Avalonia.Media.Colors.Black;
}
