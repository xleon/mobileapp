using System;

namespace Toggl.Multivac.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static long ToUnixTimeSeconds(this DateTimeOffset dateTime)
        {
            // constant and implementation taken from .NET reference source:
            // https://referencesource.microsoft.com/#mscorlib/system/datetimeoffset.cs
            const long unixEpochSeconds = 62_135_596_800L;

            var seconds = dateTime.UtcDateTime.Ticks / TimeSpan.TicksPerSecond;
            return seconds - unixEpochSeconds;
        }
    }
}
