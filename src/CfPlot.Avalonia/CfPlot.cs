using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace CfPlot.Avalonia;

public class CfPlot : Control
{
    public List<DataStreamer> DataStreams { get; } = new();

    public DataStreamer AddDataStreamer(int points)
    {
        var dataStreamer = new DataStreamer(this, Generate.NaN(points))
        {
            Color = GetNextColor()
        };
        DataStreams.Add(dataStreamer);
        return dataStreamer;
    }

    public void Refresh()
    {
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }

    public override void Render(DrawingContext context)
    {
        foreach (var dataStreamer in DataStreams)
        {
            var pathGeometry = new PathGeometry();
            var figure = new PathFigure();

            if (dataStreamer.Data.CountTotal <= 0)
            {
                continue;
            }

            var xScaleFactor = Bounds.Width / dataStreamer.Data.Length;

            var startX = Bounds.Width - xScaleFactor * (dataStreamer.Data.NewestIndex + 1);
            if (startX < 0)
            {
                startX = 0;
            }

            figure.StartPoint = new Point(startX, dataStreamer.Data.Data[0]);

            var polyLineSegment = new PolyLineSegment();
            var points = new Points();

            for (var i = 0; i <= dataStreamer.Data.NewestIndex; i++)
            {
                var x = startX + i * xScaleFactor;
                var y = dataStreamer.Data.Data[i];
                points.Add(new Point(x, y));
            }

            polyLineSegment.Points = points;
            figure.Segments?.Add(polyLineSegment);
            pathGeometry.Figures?.Add(figure);

            var pen = new Pen(dataStreamer.Color.ToSolidColorBrush(), 2);

            context.DrawGeometry(null, pen, pathGeometry);
        }
    }

    public IPalette Palette { get; set; } = new Palettes.Category10();
    private int NextColorIndex = 0;

    public Color GetNextColor(bool incrementCounter = true)
    {
        if (DataStreams.Count == 0)
            NextColorIndex = 0;

        var color = Palette.GetColor(NextColorIndex);

        if (incrementCounter)
            NextColorIndex++;

        return color;
    }
}