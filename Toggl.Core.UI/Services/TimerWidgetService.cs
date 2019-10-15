using System;
using System.Collections.Immutable;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Shared;

namespace Toggl.Core.UI.Services
{
    public abstract class TimerWidgetService : ITimerWidgetService
    {
        private readonly ITogglDataSource dataSource;
        private IDisposable runningTimeEntryDisposable;

        protected TimerWidgetService(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public void Start()
        {
            if (runningTimeEntryDisposable != null)
            {
                runningTimeEntryDisposable?.Dispose();
                runningTimeEntryDisposable = null;
            }

            runningTimeEntryDisposable = dataSource
                .TimeEntries
                .CurrentlyRunningTimeEntry
                .Subscribe(OnRunningTimeEntryChanged);
        }

        public abstract void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry);

        public abstract void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions);
    }
}
