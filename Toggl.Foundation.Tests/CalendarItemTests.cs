using System;
using FluentAssertions;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.Tests.Mocks;
using Toggl.PrimeRadiant;
using Xunit;

namespace Toggl.Foundation.Tests
{
    public sealed class CalendarItemTests
    {
        public sealed class TheFromTimeEntryMethod
        {
            [Theory, LogIfTooSlow]
            [InlineData(SyncStatus.InSync, CalendarIconKind.None)]
            [InlineData(SyncStatus.RefetchingNeeded, CalendarIconKind.None)]
            [InlineData(SyncStatus.SyncFailed, CalendarIconKind.Unsyncable)]
            [InlineData(SyncStatus.SyncNeeded, CalendarIconKind.Unsynced)]
            public void SetsTheAppropriateCalendarIconKind(SyncStatus syncStatus, CalendarIconKind expectedCalendarIcon)
            {
                var timeEntry = new MockTimeEntry { SyncStatus = syncStatus };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.IconKind.Should().Be(expectedCalendarIcon);
            }
        }
    }
}
