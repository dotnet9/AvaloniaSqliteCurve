namespace ConsoleApp1;

public enum FormatType
{
    None,
    NineFour,
    EightThree,
    SevenTwo
}

public static class FormatTypeExtension
{
    public static string Format(this double value, FormatType format)
    {
        switch (format)
        {
            case FormatType.NineFour:
                var absValue = Math.Abs(value);
                return absValue switch
                {
                    > 99999 => value.ToString("#####.####e+00"),
                    < 1 and >= 0.0001 => value.ToString("0.####"),
                    < 0.0001 => value.ToString("0.####e+00"),
                    _ => value.ToString("####0.####")
                };
            case FormatType.EightThree:
                return Math.Abs(value) switch
                {
                    > 99999 => value.ToString("#####.###e+00"),
                    < 1 and >= 0.001 => value.ToString("0.###"),
                    < 0.001 => value.ToString("0.###e+00"),
                    _ => value.ToString("####0.###")
                };
            case FormatType.SevenTwo:
                return Math.Abs(value) switch
                {
                    > 99999 => value.ToString("#####.##e+00"),
                    < 1 and >= 0.01 => value.ToString("0.##"),
                    < 0.01 => value.ToString("0.##e+00"),
                    _ => value.ToString("####0.##")
                };
            default:
                return value.ToString("#########0.#################");
        }
    }

    public static string FormatNumber(this int value, FormatType format)
    {
        return Format((double)value, format);
    }
}