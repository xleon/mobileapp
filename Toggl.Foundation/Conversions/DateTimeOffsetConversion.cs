using System;

namespace Toggl.Foundation.Conversions
{
    public static class DateTimeOffsetConversion
    {
        private static readonly string[] dayOfWeekInitials =
        {
            Resources.SundayInitial,
            Resources.MondayInitial,
            Resources.TuesdayInitial,
            Resources.WednesdayInitial,
            Resources.ThursdayInitial,
            Resources.FridayInitial,
            Resources.SaturdayInitial
        };

        public static string ToDayOfWeekInitial(DateTimeOffset date)
            => dayOfWeekInitials[(int)date.DayOfWeek];
    }
}
