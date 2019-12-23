using System;
using Toggl.Shared.Extensions;

namespace Toggl.Shared
{
    public struct DateRange : IEquatable<DateRange>
    {
        public DateRange(DateTime beginning, DateTime end)
        {
            if (end < beginning)
                (beginning, end) = (end, beginning);

            Beginning = beginning;
            End = end;
        }

        public static DateRange MonthFrom(DateTime day)
            => new DateRange(day.FirstDayOfSameMonth(), day.LastDayOfSameMonth());

        public DateTime Beginning { get; }
        public DateTime End { get; }

        public int Length => 1 + (int)(End - Beginning).TotalDays;

        public DateTimeOffsetRange ToLocalInstantaneousTimeRange()
        {
            var localOffset = DateTimeOffset.Now.Offset;
            var beginning = new DateTimeOffset(Beginning, localOffset);
            var end = new DateTimeOffset(End, localOffset);
            return new DateTimeOffsetRange(beginning, end);
        }

        public bool Contains(DateRange range)
            => Beginning <= range.Beginning && range.End <= End;

        public bool Contains(DateTime date)
            => Beginning <= date && date <= End;

        public bool IsContainedIn(DateRange range)
            => range.Beginning <= Beginning && End <= range.End;

        public bool OverlapsWith(DateRange range)
            => Beginning <= range.End && End >= range.Beginning;

        public bool IsSingleDay
            => Beginning == End;

        public bool Equals(DateRange other)
            => Beginning == other.Beginning && End == other.End;

        public override bool Equals(object? obj)
            => obj is DateRange date
                ? Equals(date)
                : false;

        public static bool operator ==(DateRange range, DateRange other)
            => range.Equals(other);

        public static bool operator !=(DateRange range, DateRange other)
            => !range.Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(Beginning, End);

        public override string ToString()
            => $"[{Beginning}, {End}]";
    }
}
