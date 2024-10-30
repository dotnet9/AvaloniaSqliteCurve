namespace CfPlot.Avalonia.Palettes;

public class Category10 : IPalette
{
    public string Name { get; } = "Category 10";

    public string Description { get; } = "A set of 10 unque colors used in " +
                                         "many data visualization libraries such as Matplotlib, Vega, and Tableau";

    public Color[] Colors { get; } = Color.FromHex(HexColors);

    private static readonly string[] HexColors =
    {
        "#1f77b4", "#ff7f0e", "#2ca02c", "#d62728", "#9467bd",
        "#8c564b", "#e377c2", "#7f7f7f", "#bcbd22", "#17becf",
    };

    public Color GetColor(int index)
    {
        return Colors[index % Colors.Length];
    }
}
