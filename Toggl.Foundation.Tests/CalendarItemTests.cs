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
        public sealed class TheConstructor
        {
            private CalendarItem createCalendarItem()
                => new CalendarItem(
                    CalendarItemSource.Calendar,
                    new DateTimeOffset(12, TimeSpan.Zero),
                    TimeSpan.FromHours(1),
                    ""
                );

            [Fact, LogIfTooSlow]
            public void SetsIsSyncedToTrue()
            {
                var calendarItem = createCalendarItem();

                calendarItem.IsSynced.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void SetsCanBeSyncedToTrue()
            {
                var calendarItem = createCalendarItem();

                calendarItem.CanBeSynced.Should().BeTrue();
            }
        }

        public sealed class TheFromTimeEntryMethod
        {
            [Theory, LogIfTooSlow]
            [InlineData(SyncStatus.InSync, true)]
            [InlineData(SyncStatus.RefetchingNeeded, false)]
            [InlineData(SyncStatus.SyncFailed, false)]
            [InlineData(SyncStatus.SyncNeeded, false)]
            public void SetsIsSyncedToTrueIfTimeEntryIsInSyncAndFalseFoAllOthercases(
                SyncStatus syncStatus, bool expectedIsSynced)
            {
                var timeEntry = new MockTimeEntry
                {
                    SyncStatus = syncStatus,
                };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.IsSynced.Should().Be(expectedIsSynced);
            }

            [Theory, LogIfTooSlow]
            [InlineData(SyncStatus.InSync, true)]
            [InlineData(SyncStatus.RefetchingNeeded, true)]
            [InlineData(SyncStatus.SyncFailed, false)]
            [InlineData(SyncStatus.SyncNeeded, true)]
            public void SetsCanBeSyncedToTrueIfSyncHasNotFailed(
                SyncStatus syncStatus, bool expectedCanBeSynced)
            {
                var timeEntry = new MockTimeEntry
                {
                    SyncStatus = syncStatus
                };

                var calendarItem = CalendarItem.From(timeEntry);

                calendarItem.CanBeSynced.Should().Be(expectedCanBeSynced);
            }
        }
    }
}
