namespace CfPlot.Avalonia.Palettes;

public class ColorblindFriendly : IPalette
{
    public string Name { get; } = "Colorblind Friendly";

    public string Description { get; } = "A set of 8 colorblind-friendly colors " +
                                         "from Bang Wong's Nature Methods paper https://www.nature.com/articles/nmeth.1618.pdf";

    public Color[] Colors { get; } = Color.FromHex(HexColors);

    private static readonly string[] HexColors =
    {
        "#000000", "#E69F00", "#56B4E9", "#009E73", "#F0E442",
        "#0072B2", "#D55E00", "#CC79A7",
    };

    public Color GetColor(int index)
    {
        return Colors[index % Colors.Length];
    }
}