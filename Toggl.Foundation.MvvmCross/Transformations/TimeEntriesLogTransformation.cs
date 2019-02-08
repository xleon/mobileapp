using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.Transformations
{
    internal static class TimeEntriesLogTransformation
    {
        public static IImmutableList<CollectionSection<DaySummaryViewModel, CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>> Group(
            IEnumerable<IThreadSafeTimeEntry> timeEntries, IThreadSafePreferences preferences, DateTimeOffset currentDateTime)
            => timeEntries
                .Where(isNotRunning)
                .Select(te => new TimeEntryViewModel(te, preferences.DurationFormat))
                .OrderByDescending(te => te.StartTime)
                .GroupBy(te => te.StartTime.LocalDateTime.Date)
                .Select(group => transform(group, preferences, currentDateTime))
                .ToImmutableList();

        private static bool isNotRunning(IThreadSafeTimeEntry timeEntry) => !timeEntry.IsRunning();

        private static CollectionSection<DaySummaryViewModel, CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>> transform(
            IGrouping<DateTime, TimeEntryViewModel> dayGroup, IThreadSafePreferences preferences, DateTimeOffset currentDateTime)
            => new CollectionSection<DaySummaryViewModel, CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>(
                new DaySummaryViewModel(
                    DateToTitleString.Convert(dayGroup.Key, currentDateTime),
                    totalTrackedTime(dayGroup).ToFormattedString(preferences.DurationFormat)),
                toGroups(dayGroup, preferences));

        private static TimeSpan totalTrackedTime(IEnumerable<TimeEntryViewModel> timeEntries)
            => timeEntries.Sum(timeEntry => timeEntry.Duration);

        private static IEnumerable<CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>> toGroups(
            IEnumerable<TimeEntryViewModel> timeEntries, IThreadSafePreferences preferences)
            => groupAccordingToPreferences(timeEntries, preferences)
                .OrderByDescending(group => group.Max(timeEntry => timeEntry.StartTime))
                .Select(group => group.ToArray())
                .Select(group =>
                    new CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>(
                        new TimeEntriesGroupViewModel(group), group));

        private static IEnumerable<IEnumerable<TimeEntryViewModel>> groupAccordingToPreferences(IEnumerable<TimeEntryViewModel> timeEntries, IThreadSafePreferences preferences)
            => preferences.CollapseTimeEntries
                ? groupTimeEntriesBySimilarity(timeEntries)
                : singleTimeEntryGroups(timeEntries);

        private static IEnumerable<IEnumerable<TimeEntryViewModel>> groupTimeEntriesBySimilarity(IEnumerable<TimeEntryViewModel> timeEntries)
            => timeEntries.GroupBy(timeEntry => timeEntry, new TimeEntriesComparer());

        private static IEnumerable<IEnumerable<TimeEntryViewModel>> singleTimeEntryGroups(IEnumerable<TimeEntryViewModel> timeEntries)
            => timeEntries.Select(timeEntry => new[] { timeEntry });

        private sealed class TimeEntriesComparer : IEqualityComparer<TimeEntryViewModel>
        {
            public bool Equals(TimeEntryViewModel x, TimeEntryViewModel y)
                => x != null
                   && y != null
                   && x.WorkspaceId == y.WorkspaceId
                   && x.Description == y.Description
                   && x.ProjectId == y.ProjectId
                   && x.TaskId == y.TaskId
                   && x.IsBillable == y.IsBillable
                   && haveSameTags(x.TagIds, y.TagIds);

            public int GetHashCode(TimeEntryViewModel timeEntry)
            {
                var hashCode = HashCode.From(
                    timeEntry.WorkspaceId,
                    timeEntry.Description,
                    timeEntry.ProjectId,
                    timeEntry.TaskId,
                    timeEntry.IsBillable);

                var tags = timeEntry.TagIds.OrderBy(id => id);
                foreach (var tag in tags)
                {
                    hashCode = HashCode.From(hashCode, tag);
                }

                return hashCode;
            }

            private static bool haveSameTags(IReadOnlyCollection<long> a, IReadOnlyCollection<long> b)
                => (a == null && b == null)
                   || (a?.Count == b?.Count && a.OrderBy(id => id).SequenceEqual(b.OrderBy(id => id)));
        }
    }
}
