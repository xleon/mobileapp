using System;

namespace Toggl.Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime BeginningOfWeek(this DateTime time, BeginningOfWeek beginningOfWeek)
        {
            var offset = (7 + (int)time.DayOfWeek - (int)beginningOfWeek) % 7;
            return time.Date.AddDays(-offset);
        }

        public static int DaysInMonth(this DateTime time)
            => DateTime.DaysInMonth(time.Year, time.Month);

        public static DateTime LastDayOfSameMonth(this DateTime time)
        {
            var day = DateTime.DaysInMonth(time.Year, time.Month);
            return new DateTime(time.Year, time.Month, day);
        }

        public static DateTime FirstDayOfSameMonth(this DateTime time)
            => new DateTime(time.Year, time.Month, 1);
    }
}
