using System;
using Toggl.Core.Extensions;
using Toggl.Shared;

namespace Toggl.Core.MvvmCross.Transformations
{
    public class DurationAndFormatToString
    {
        public static string Convert(TimeSpan duration, DurationFormat format)
        {
            return duration.ToFormattedString(format);
        }
    }
}
