using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.ExtensionKit;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class TimerWidgetService
    {
        private IDisposable runningTimeEntryDisposable;
        private IDisposable durationFormatDisposable;

        public void Start(ITogglDataSource dataSource)
        {
            Stop();

            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(onRunningTimeEntry);

            durationFormatDisposable = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.DurationFormat)
                .Subscribe(onDurationFormat);
        }

        public void Stop()
        {
            runningTimeEntryDisposable?.Dispose();
            runningTimeEntryDisposable = null;

            durationFormatDisposable?.Dispose();
            durationFormatDisposable = null;
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

        private void onDurationFormat(DurationFormat durationFormat)
        {
            SharedStorage.Instance.SetDurationFormat((int) durationFormat);
        }
    }
}
