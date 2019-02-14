using System;
using System.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.TimeEntriesLog
{
    public sealed class TimeEntriesGroupViewModel
    {
        public int GroupSize { get; }
        public bool IsExpandable { get; }
        public TimeEntryViewModel Summary { get; }

        public TimeEntriesGroupViewModel(TimeEntryViewModel[] timeEntries)
        {
            if (timeEntries.Length == 0)
                throw new InvalidOperationException($"{nameof(TimeEntriesGroupViewModel)} cannot be empty.");

            var sample = timeEntries.First();

            GroupSize = timeEntries.Length;
            IsExpandable = timeEntries.Length > 1;
            Summary = new TimeEntryViewModel(
                id: 0,
                isBillable: sample.IsBillable,
                description: sample.Description,
                startTime: timeEntries.Min(timeEntry => timeEntry.StartTime),
                duration: timeEntries.Sum(timeEntry => timeEntry.Duration),
                workspaceId: sample.WorkspaceId,
                projectId: sample.ProjectId,
                projectName: sample.ProjectName,
                projectColor: sample.ProjectColor,
                clientName: sample.ClientName,
                taskId: sample.TaskId,
                taskName: sample.TaskName,
                tagIds: sample.TagIds,
                needsSync: timeEntries.Any(timeEntry => timeEntry.NeedsSync),
                canSync: timeEntries.All(timeEntry => timeEntry.CanSync),
                isInaccessible: sample.IsInaccessible,
                durationFormat: sample.DurationFormat);
        }
    }
}
