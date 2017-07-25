using System;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class DateParameter
    {
        public string DateString { get; set; }

        public static DateParameter WithDate(DateTimeOffset date) => new DateParameter
        {
            DateString = date.ToString()
        };
    }

    internal static class DateParameterExtensions
    {
        public static DateTimeOffset GetDate(this DateParameter self)
            => DateTimeOffset.Parse(self.DateString);
    }
}
