namespace AvaloniaSqliteCurve.Models;

public class ConstData
{
    public const int LineCount = 8;
    public const int DisplayMaxPointsCount = 3000;
    public const double MinBottom = -300.0;
    public const double MaxTop = 300.0;

    public const int AddDataInterval = 1000;
    public const int UpdateDataInterval = 1000;


    public static Avalonia.Media.Color Fill = Avalonia.Media.Colors.White;
    public static Avalonia.Media.Color Stroke = Avalonia.Media.Colors.Black;
}
