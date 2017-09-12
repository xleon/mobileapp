using System;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class DurationParameter
    {
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop { get; set; }

        public static DurationParameter WithStartAndStop(DateTimeOffset start, DateTimeOffset? stop)
            => new DurationParameter { Start = start, Stop = stop };
    }
}
