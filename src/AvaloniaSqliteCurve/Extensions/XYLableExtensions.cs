namespace AvaloniaSqliteCurve.Extensions;

internal static class XYLableExtensions
{
    private const int MinutesInHour = 60;
    private const int MinutesInDay = 1440;

    internal static string GetTimeStr(double minutes, double maxMinutes)
    {
        // 整个翻译大于1小时，分割时间小于1小时
        if (maxMinutes >= MinutesInHour && minutes < MinutesInHour)
        {
            var wholeMinutes = (int)minutes;
            return $"0:{wholeMinutes:D2}";
        }

        switch (minutes)
        {
            case < MinutesInHour:
            {
                var wholeMinutes = (int)minutes;
                var seconds = (int)((minutes - wholeMinutes) * 60);
                return $"{wholeMinutes}:{seconds:D2}";
            }
            case >= MinutesInHour and < MinutesInDay:
            {
                var hours = (int)(minutes / MinutesInHour);
                var remainingMinutes = (int)(minutes % MinutesInHour);
                return $"{hours}:{remainingMinutes:D2}";
            }
            default:
            {
                var days = (int)(minutes / MinutesInDay);
                var remainingMinutes = (int)(minutes % MinutesInDay);
                var hours = remainingMinutes / MinutesInHour;
                return $"{days}:{hours:D2}";
            }
        }
    }

    internal static string GetTimeUnit(double minutes)
    {
        return minutes switch
        {
            < MinutesInHour => "Min",
            < MinutesInDay => "Hour",
            _ => "Day"
        };
    }
}