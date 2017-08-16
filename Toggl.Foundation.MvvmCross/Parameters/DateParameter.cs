using System;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
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
