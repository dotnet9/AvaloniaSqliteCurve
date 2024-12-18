﻿namespace ConsoleApp1;

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
        var absValue = Math.Abs(value);

        return format switch
        {
            FormatType.NineFour => absValue switch
            {
                > 99999 => value.ToString("#####.####e+00"),
                < 0.0001 when absValue != 0 => value.ToString("0.####e+00"),
                _ => value.ToString("####0.####")
            },
            FormatType.EightThree => absValue switch
            {
                > 99999 => value.ToString("#####.###e+00"),
                < 0.001 when absValue != 0 => value.ToString("0.###e+00"),
                _ => value.ToString("####0.###")
            },
            FormatType.SevenTwo => absValue switch
            {
                > 99999 => value.ToString("#####.##e+00"),
                < 0.01 when absValue != 0 => value.ToString("0.##e+00"),
                _ => value.ToString("####0.##")
            },
            _ => value.ToString("#################0.#################")
        };
    }

    public static string Format(this int value, FormatType format)
    {
        return Format((double)value, format);
    }
}