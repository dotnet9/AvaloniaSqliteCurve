﻿using System;

namespace AvaloniaSqliteCurve.Helpers
{
    public static class TimestampHelper
    {
        public static long ToUtcTimestamp(this DateTime dateTime)
        {
            var time = new DateTime(1970, 1, 1);
            var ts = dateTime - time;
            return (long)ts.TotalMilliseconds;
        }
        public static int ToTodayTimestamp(this DateTime dateTime)
        {
            var time = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            var ts = dateTime - time;
            return (int)ts.TotalMilliseconds;
        }

        public static DateTime ToUtcDateTime(this long milliseconds)
        {
            var time = new DateTime(1970, 1, 1);
            var dt = time.AddMilliseconds(milliseconds);
            return dt;
        }
    }
}