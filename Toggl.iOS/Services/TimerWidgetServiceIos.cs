using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Services;
using Toggl.iOS.ExtensionKit;
using Toggl.Shared;

namespace Toggl.iOS.Services
{
    public sealed class TimerWidgetServiceIos : TimerWidgetService
    {
        private IDisposable durationFormatDisposable;

        public TimerWidgetServiceIos(ITogglDataSource dataSource) : base(dataSource)
        {
            durationFormatDisposable = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.DurationFormat)
                .Subscribe(onDurationFormat);
        }

        protected override void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry)
        {
            if (timeEntry == null)
            {
                SharedStorage.Instance.SetRunningTimeEntry(null);
                return;
            }

            SharedStorage.Instance.SetRunningTimeEntry(
                timeEntry,
                timeEntry.Project?.Name ?? "",
                timeEntry.Project?.Color ?? "",
                timeEntry.Task?.Name ?? "",
                timeEntry.Project?.Client?.Name ?? "");
        }

        private void onDurationFormat(DurationFormat durationFormat)
        {
            SharedStorage.Instance.SetDurationFormat((int) durationFormat);
        }
    }
}
