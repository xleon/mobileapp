using System;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.UI.Services
{
    public abstract class TimerWidgetService
    {
        private IDisposable runningTimeEntryDisposable;

        protected TimerWidgetService(ITogglDataSource dataSource)
        {
            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(OnRunningTimeEntryChanged);
        }

        protected abstract void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry);
    }
}
