using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Transformations;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog;
using Toggl.Foundation.Tests.Mocks;
using Toggl.Multivac;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Transformations
{
    public sealed class TimeEntriesLogTrasnformationTests
    {
        private static readonly DateTimeOffset now = new DateTimeOffset(2019, 02, 07, 16, 25, 38, TimeSpan.FromHours(-1));
        private static readonly IThreadSafeWorkspace workspaceA = new MockWorkspace { Id = 1, IsInaccessible = false };
        private static readonly IThreadSafeWorkspace workspaceB = new MockWorkspace { Id = 2, IsInaccessible = false };
        private static readonly IThreadSafeProject projectA = new MockProject { Id = 1, Name = "ProjectA", Color = "blue", Active = true };
        private static readonly IThreadSafeProject projectB = new MockProject { Id = 2, Name = "ProjectB", Color = "red", Active = true };
        private static readonly IThreadSafeTask taskA = new MockTask { Id = 1, Name = "TaskA" };
        private static readonly IThreadSafeTask taskB = new MockTask { Id = 2, Name = "TaskB" };
        private static readonly IThreadSafeTag tagA = new MockTag { Id = 1, Name = "TagA" };
        private static readonly IThreadSafeTag tagB = new MockTag { Id = 2, Name = "TagB" };
        private static readonly IThreadSafeTag tagC = new MockTag { Id = 3, Name = "TagC" };

        private static readonly IThreadSafeTimeEntry[] singleDay =
        {
            createTimeEntry(now, workspaceA, "A", duration: 1),
            createTimeEntry(now, workspaceA, "A", duration: 2),
            createTimeEntry(now, workspaceA, "A", duration: 4)
        };

        private static readonly IThreadSafeTimeEntry[] twoWorkspaces =
        {
            createTimeEntry(now, workspaceA, "B", duration: 1),
            createTimeEntry(now, workspaceB, "B", duration: 2)
        };

        private static readonly IThreadSafeTimeEntry[] differentDescriptions =
        {
            createTimeEntry(now, workspaceA, "C1", duration: 1),
            createTimeEntry(now, workspaceA, "C1", duration: 2),
            createTimeEntry(now, workspaceA, "C2", duration: 4)
        };

        private static readonly IThreadSafeTimeEntry[] multipleDays =
        {
            createTimeEntry(now, workspaceA, "D1", duration: 1),
            createTimeEntry(now - TimeSpan.FromDays(1), workspaceA, "D1", duration: 2),
            createTimeEntry(now, workspaceA, "D2", duration: 4)
        };

        private static readonly IThreadSafeTimeEntry[] multipleDaysWithTags =
        {
            createTimeEntry(now, workspaceA, "E1", duration: 1),
            createTimeEntry(now - TimeSpan.FromDays(1), workspaceA, "E1", duration: 2),
            createTimeEntry(now, workspaceA, "E2", duration: 4),
            createTimeEntry(now, workspaceA, "E2", duration: 8, tags: new[] { tagA, tagB, tagC }),
            createTimeEntry(now, workspaceA, "E2", duration: 16, tags: new[] { tagA, tagC, tagB })
        };

        private static readonly IThreadSafeTimeEntry[] multipleDaysWithProject =
        {
            createTimeEntry(now, workspaceA, "F1", duration: 1),
            createTimeEntry(now - TimeSpan.FromDays(1), workspaceA, "F1", duration: 2),
            createTimeEntry(now, workspaceA, "F2", duration: 4, project: projectA),
            createTimeEntry(now, workspaceA, "F2", duration: 8),
            createTimeEntry(now, workspaceA, "F2", duration: 16, project: projectA)
        };

        private static readonly IThreadSafeTimeEntry[] differentProjects =
        {
            createTimeEntry(now, workspaceA, "G", duration: 1, project: projectA),
            createTimeEntry(now, workspaceA, "G", duration: 2, project: projectB)
        };

        private static readonly IThreadSafeTimeEntry[] sameTasks =
        {
            createTimeEntry(now, workspaceA, "H", duration: 1, project: projectA, task: taskA),
            createTimeEntry(now, workspaceA, "H", duration: 2, project: projectA, task: taskA)
        };

        private static readonly IThreadSafeTimeEntry[] differentTasks =
        {
            createTimeEntry(now, workspaceA, "I", duration: 1, project: projectA, task: taskA),
            createTimeEntry(now, workspaceA, "I", duration: 2, project: projectA, task: taskB)
        };

        public sealed class WhenGroupingIsEnabled
        {
            private static readonly IThreadSafePreferences preferences = new MockPreferences
            {
                CollapseTimeEntries = true,
                DateFormat = DateFormat.FromLocalizedDateFormat("YYYY-MM-DD")
            };

            [Theory]
            [MemberData(nameof(TestData))]
            public void TransformsTimeEntriesIntoACorrectTree(
                IEnumerable<IThreadSafeTimeEntry> timeEntries,
                params CollectionSection<DaySummaryViewModel,
                    CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>[] expectedTree)
            {
                var transformedTree = TimeEntriesLogTransformation.Group(timeEntries, preferences, now);

                transformedTree.Should().BeEquivalentTo(expectedTree.ToImmutableList());
            }

            private static CollectionSection<DaySummaryViewModel,
                CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>> dayOf(
                string title,
                string totalTrackedTime,
                params CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>[] groups)
                => new CollectionSection<DaySummaryViewModel,
                    CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>(
                    new DaySummaryViewModel(title, totalTrackedTime),
                    groups);

            private static CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel> groupOf(
                params IThreadSafeTimeEntry[] timeEntries)
                => new CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>(
                    new TimeEntriesGroupViewModel(toViewModels(timeEntries)),
                    toViewModels(timeEntries));

            private static TimeEntryViewModel[] toViewModels(IThreadSafeTimeEntry[] timeEntries)
                => timeEntries.Select(te => new TimeEntryViewModel(te, preferences.DurationFormat)).ToArray();

            public static IEnumerable<object[]> TestData
                => new[]
                {
                    new object[]
                    {
                        singleDay,
                        dayOf("Today", "07 sec", groupOf(singleDay))
                    },
                    new object[]
                    {
                        twoWorkspaces,
                        dayOf("Today", "03 sec", groupOf(twoWorkspaces[0]), groupOf(twoWorkspaces[1]))
                    },
                    new object[]
                    {
                        differentDescriptions,
                        dayOf("Today", "07 sec", groupOf(differentDescriptions[0], differentDescriptions[1]), groupOf(differentDescriptions[2]))
                    },
                    new object[]
                    {
                        multipleDays,
                        dayOf("Today", "05 sec", groupOf(multipleDays[0]), groupOf(multipleDays[2])),
                        dayOf("Yesterday", "02 sec", groupOf(multipleDays[1]))
                    },
                    new object[]
                    {
                        multipleDaysWithTags,

                        dayOf("Today", "29 sec", groupOf(multipleDaysWithTags[0]), groupOf(multipleDaysWithTags[2]), groupOf(multipleDaysWithTags[3], multipleDaysWithTags[4])),
                        dayOf("Yesterday", "02 sec", groupOf(multipleDaysWithTags[1]))
                    },
                    new object[]
                    {
                        multipleDaysWithProject,
                        dayOf("Today", "29 sec", groupOf(multipleDaysWithProject[0]), groupOf(multipleDaysWithProject[2], multipleDaysWithProject[4]), groupOf(multipleDaysWithProject[3])),
                        dayOf("Yesterday", "02 sec", groupOf(multipleDaysWithProject[1]))
                    },
                    new object[]
                    {
                        differentProjects,
                        dayOf("Today", "03 sec", groupOf(differentProjects[0]), groupOf(differentProjects[1]))
                    },
                    new object[]
                    {
                        differentTasks,
                        dayOf("Today", "03 sec", groupOf(differentTasks[0]), groupOf(differentTasks[1]))
                    },
                    new object[]
                    {
                        sameTasks,
                        dayOf("Today", "03 sec", groupOf(sameTasks[0], sameTasks[1]))
                    }
                };
        }

        public sealed class WhenGroupingIsDisabled
        {
            private static readonly IThreadSafePreferences preferences = new MockPreferences
            {
                CollapseTimeEntries = false,
                DateFormat = DateFormat.FromLocalizedDateFormat("YYYY-MM-DD")
            };

            [Theory]
            [MemberData(nameof(TestData))]
            public void TransformsTimeEntriesIntoACorrectTree(
                IEnumerable<IThreadSafeTimeEntry> timeEntries,
                params CollectionSection<DaySummaryViewModel,
                    CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>[] expectedTree)
            {
                var transformedTree = TimeEntriesLogTransformation.Group(timeEntries, preferences, now);

                transformedTree.Should().BeEquivalentTo(expectedTree.ToImmutableList());
            }

            private static CollectionSection<DaySummaryViewModel,
                CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>> dayOf(
                string title,
                string totalTrackedTime,
                params CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>[] groups)
                => new CollectionSection<DaySummaryViewModel,
                    CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>>(
                    new DaySummaryViewModel(title, totalTrackedTime),
                    groups);

            private static CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>[] groupsOf(
                params IThreadSafeTimeEntry[] timeEntries)
                => timeEntries.Select(timeEntry =>
                    new CollectionSection<TimeEntriesGroupViewModel, TimeEntryViewModel>(
                        new TimeEntriesGroupViewModel(toViewModel(timeEntry)),
                        toViewModel(timeEntry))).ToArray();

            private static TimeEntryViewModel[] toViewModel(IThreadSafeTimeEntry timeEntry)
                => new[] { new TimeEntryViewModel(timeEntry, preferences.DurationFormat) };

            public static IEnumerable<object[]> TestData
                => new[]
                {
                    new object[]
                    {
                        singleDay,
                        dayOf("Today", "07 sec", groupsOf(singleDay))
                    },
                    new object[]
                    {
                        twoWorkspaces,
                        dayOf("Today", "03 sec", groupsOf(twoWorkspaces))
                    },
                    new object[]
                    {
                        differentDescriptions,
                        dayOf("Today", "07 sec", groupsOf(differentDescriptions))
                    },
                    new object[]
                    {
                        multipleDays,
                        dayOf("Today", "05 sec", groupsOf(multipleDays[0], multipleDays[2])),
                        dayOf("Yesterday", "02 sec", groupsOf(multipleDays[1]))
                    },
                    new object[]
                    {
                        multipleDaysWithTags,

                        dayOf("Today", "29 sec", groupsOf(multipleDaysWithTags[0], multipleDaysWithTags[2], multipleDaysWithTags[3], multipleDaysWithTags[4])),
                        dayOf("Yesterday", "02 sec", groupsOf(multipleDaysWithTags[1]))
                    },
                    new object[]
                    {
                        multipleDaysWithProject,
                        dayOf("Today", "29 sec", groupsOf(multipleDaysWithProject[0], multipleDaysWithProject[2], multipleDaysWithProject[3], multipleDaysWithProject[4])),
                        dayOf("Yesterday", "02 sec", groupsOf(multipleDaysWithProject[1]))
                    },
                    new object[]
                    {
                        differentProjects,
                        dayOf("Today", "03 sec", groupsOf(differentProjects))
                    },
                    new object[]
                    {
                        differentTasks,
                        dayOf("Today", "03 sec", groupsOf(differentTasks))
                    },
                    new object[]
                    {
                        sameTasks,
                        dayOf("Today", "03 sec", groupsOf(sameTasks))
                    }
                };
        }

        private static IThreadSafeTimeEntry createTimeEntry(
            DateTimeOffset start,
            IThreadSafeWorkspace workspace,
            string description,
            long duration,
            IThreadSafeProject project = null,
            IThreadSafeTask task = null,
            IThreadSafeTag[] tags = null,
            bool billable = false)
            => new MockTimeEntry
            {
                Start = start,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                Description = description,
                Duration = duration,
                Project = project,
                ProjectId = project?.Id,
                Task = task,
                TaskId = task?.Id,
                Billable = billable,
                Tags = tags,
                TagIds = tags?.Select(tag => tag.Id) ?? new long[0]
            };
    }
}
