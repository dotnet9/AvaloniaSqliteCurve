// ReSharper disable once CheckNamespace
namespace CfPlot.Avalonia;

public readonly struct LinePattern(float[] intervals, float phase, string name)
{
    public static LinePattern Solid { get; } = new([], 0, "Solid");
    public static LinePattern Dashed { get; } = new([10, 10], 0, "Dashed");
    public static LinePattern DenselyDashed { get; } = new([6, 6], 0, "DenselyDashed");
    public static LinePattern Dotted { get; } = new([3, 5], 0, "Dotted");

    public float[] Intervals { get; } = intervals;
    public float Phase { get; } = phase;
    public string Name { get; } = name;

    public static LinePattern[] GetAllPatterns()
    {
        return typeof(LinePattern)
            .GetProperties()
            .Select(property => property.GetValue(Solid))
            .OfType<LinePattern>()
            .ToArray();
    }
}