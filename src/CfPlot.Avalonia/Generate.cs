namespace CfPlot.Avalonia;

public class Generate
{
    public static RandomDataGenerator RandomData { get; } = new(0);

    public static double[] Repeating(int count, double value)
    {
        var numArray = new double[count];
        for (var index = 0; index < numArray.Length; ++index)
            numArray[index] = value;
        return numArray;
    }

    public static double RandomNumber()
    {
        return RandomData.RandomNumber(0, 1);
    }

    public static double[] NaN(int count) => Generate.Repeating(count, double.NaN);
}