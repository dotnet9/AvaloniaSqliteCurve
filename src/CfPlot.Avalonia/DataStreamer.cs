namespace CfPlot.Avalonia;

public class DataStreamer(CfPlot plot, double[] data)
{
    private readonly CfPlot Plot = plot;

    public LineStyle LineStyle { get; set; } = new() { Width = 1 };

    public float LineWidth
    {
        get => LineStyle.Width;
        set => LineStyle.Width = value;
    }

    public LinePattern LinePattern
    {
        get => LineStyle.Pattern;
        set => LineStyle.Pattern = value;
    }

    public Color LineColor
    {
        get => LineStyle.Color;
        set => LineStyle.Color = value;
    }

    public Color Color
    {
        get => LineStyle.Color;
        set => LineStyle.Color = value;
    }

    public DataStreamerSource Data { get; set; } = new(data);
}