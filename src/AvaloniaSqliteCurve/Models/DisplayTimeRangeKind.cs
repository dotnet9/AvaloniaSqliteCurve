using System.ComponentModel;

namespace AvaloniaSqliteCurve.Models;

internal enum DisplayTimeRangeKind
{
    [Description("5分钟")] FiveMinutes = 5,
    [Description("10分钟")] TenMinutes = 10,
    [Description("30分钟")] ThirtyMinutes = 30,
    [Description("1小时")] OneHour = 60,
    [Description("2小时")] TwoHours = 120,
    [Description("4小时")] FourHours = 240,
    [Description("8小时")] EightHours = 480,
    [Description("1天")] OneDay = 1440,
    [Description("2天")] TwoDays = 2880,
}