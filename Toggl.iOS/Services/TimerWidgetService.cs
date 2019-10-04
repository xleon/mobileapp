using System;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.ExtensionKit;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class TimerWidgetService
    {
        private IDisposable runningTimeEntryDisposable;

        public void Start(ITogglDataSource dataSource)
        {
            Stop();

            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(onRunningTimeEntry);
        }

        public void Stop()
        {
            runningTimeEntryDisposable?.Dispose();
            runningTimeEntryDisposable = null;
        }

        private void onRunningTimeEntry(IThreadSafeTimeEntry runningTimeEntry)
        {
            if (runningTimeEntry == null)
            {
                SharedStorage.Instance.SetRunningTimeEntry(null);
                return;
            }

            SharedStorage.Instance.SetRunningTimeEntry(
                runningTimeEntry,
                runningTimeEntry.Project?.Name ?? "",
                runningTimeEntry.Project?.Color ?? "",
                runningTimeEntry.Task?.Name ?? "",
                runningTimeEntry.Project?.Client?.Name ?? "");
        }
    }
}
