using System;
using Toggl.Multivac;
using FoundationResources = Toggl.Foundation.Resources;

namespace Toggl.Foundation.Extensions
{
    public static class CalendarNotificationsOptionExtensions
    {
        public static string Title(this CalendarNotificationsOption option)
        {
            switch (option)
            {
                case CalendarNotificationsOption.Disabled:
                    return FoundationResources.Disabled;
                case CalendarNotificationsOption.WhenEventStarts:
                    return FoundationResources.WhenEventStarts;
                case CalendarNotificationsOption.FiveMinutes:
                    return FoundationResources.FiveMinutes;
                case CalendarNotificationsOption.TenMinutes:
                    return FoundationResources.TenMinutes;
                case CalendarNotificationsOption.FifteenMinutes:
                    return FoundationResources.FifteenMinutes;
                case CalendarNotificationsOption.ThirtyMinutes:
                    return FoundationResources.ThirtyMinutes;
                case CalendarNotificationsOption.OneHour:
                    return FoundationResources.OneHour;
            }
            return "";
        }

        public static TimeSpan Duration(this CalendarNotificationsOption option)
        {
            switch (option)
            {
                case CalendarNotificationsOption.WhenEventStarts:
                    return TimeSpan.Zero;
                case CalendarNotificationsOption.FiveMinutes:
                    return TimeSpan.FromMinutes(5);
                case CalendarNotificationsOption.TenMinutes:
                    return TimeSpan.FromMinutes(10);
                case CalendarNotificationsOption.FifteenMinutes:
                    return TimeSpan.FromMinutes(15);
                case CalendarNotificationsOption.ThirtyMinutes:
                    return TimeSpan.FromMinutes(30);
                case CalendarNotificationsOption.OneHour:
                    return TimeSpan.FromHours(1);
            }
            return TimeSpan.Zero;
        }
    }
}
