namespace CfPlot.Avalonia;

public class DataStreamerSource(double[] data)
{
    public double[] Data { get; } = data;
    public int NextIndex { get; private set; }
    public int NewestIndex { get; private set; }
    public double NewestPoint => Data[NewestIndex];
    public int Length => Data.Length;
    public int CountTotal { get; private set; }
    public double DataMin { get; private set; } = double.PositiveInfinity;
    public double DataMax { get; private set; } = double.NegativeInfinity;

    public void Add(double value)
    {
        Data[NextIndex] = value;
        ++NextIndex;

        if (NextIndex >= Data.Length)
            NextIndex = 0;

        NewestIndex = NextIndex - 1;

        if (NewestIndex < 0)
            NewestIndex = Data.Length - 1;

        DataMin = Math.Min(value, DataMin);
        DataMax = Math.Max(value, DataMax);

        ++CountTotal;
    }

    public void AddRange(IEnumerable<double> values)
    {
        foreach (double num in values)
            Add(num);
    }

    public void Clear(double value = 0.0)
    {
        for (int index = 0; index < Data.Length; ++index)
            Data[index] = 0.0;
        DataMin = value;
        DataMax = value;
        NewestIndex = 0;
        NextIndex = 0;
        CountTotal = 0;
    }
}