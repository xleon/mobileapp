using System;
using System.Collections.Generic;
using System.Globalization;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;

namespace Toggl.Core.UI.Transformations
{
    public class DateTimeToFormattedString
    {
        public static string Convert(
            DateTimeOffset date,
            string format,
            IAnalyticsService analyticsService,
            TimeZoneInfo timeZoneInfo = null)
        {
            if (timeZoneInfo == null)
            {
                timeZoneInfo = TimeZoneInfo.Local;
            }

            return getDateTimeOffsetInCorrectTimeZone(date, timeZoneInfo, analyticsService).ToString(format, CultureInfo.InvariantCulture);
        }

        private static DateTimeOffset getDateTimeOffsetInCorrectTimeZone(
            DateTimeOffset value,
            TimeZoneInfo timeZone,
            IAnalyticsService analyticsService)
        {
            try
            {
                return value == default(DateTimeOffset) ? value : TimeZoneInfo.ConvertTime(value, timeZone);
            }
            catch (ArgumentOutOfRangeException argumentOutOfRangeException)
            {
                var exceptionProperties = new Dictionary<string, string>
                {
                    { "DateTimeOffset", value.ToString() },
                    { "TimeZoneName", timeZone.DisplayName },
                    { "TimeZoneOffset", timeZone.BaseUtcOffset.ToString() }
                };
                var exception = new UnrepresentableDateException("Unrepresentable DateTimeOffset when converting to local timezone", argumentOutOfRangeException);
                analyticsService.Track(exception, exceptionProperties);

                throw exception;
            }
        }
    }
}
