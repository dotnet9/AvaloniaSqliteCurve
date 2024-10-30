namespace CfPlot.Avalonia;

public class RandomDataGenerator
{
    private Random Rand;

    public RandomDataGenerator(int? seed = null)
    {
        Rand = seed.HasValue
            ? new Random(seed.Value)
#if NET5_0_OR_GREATER
            : Random.Shared;
#else
            : GlobalRandomThread.Value!;
#endif
    }

    public void Seed(int seed)
    {
        Rand = new(seed);
    }

    public double RandomNumber(double min, double max)
    {
        return Rand.NextDouble() * (max - min) + min;
    }
}