namespace CfPlot.Avalonia.Plottables;

public class DataStreamer(Plot plot, double[] data)
{
    private readonly Plot Plot = plot;

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

    public void Add(double value)
    {
        Data.Add(value);
    }

    public void Add(double[] ys)
    {
        if (ys is null)
            throw new ArgumentException($"{nameof(ys)} must not be null");

        for (int i = 0; i < ys.Length; i++)
        {
            Data.Add(ys[i]);
        }
    }

    public void AddRange(IEnumerable<double> values)
    {
        Data.AddRange(values);
    }

    public void Clear(double value = 0)
    {
        Data.Clear(value);
    }
}