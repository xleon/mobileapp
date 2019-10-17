using System.Collections.Immutable;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Services;
using Toggl.Droid.Helper;
using Toggl.Droid.Widgets;

namespace Toggl.Droid.Services
{
    public sealed class TimerWidgetServiceAndroid : TimerWidgetService
    {
        public TimerWidgetServiceAndroid(ITogglDataSource dataSource) : base(dataSource)
        {
        }

        public override void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry)
        {
            TimeEntryWidgetInfo.Save(timeEntry);
            AppWidgetProviderUtils.UpdateAllInstances<TimeEntryWidget>();
        }

        public override void OnSuggestionsUpdated(IImmutableList<Suggestion> suggestions)
        {
            // TODO: Update the widget
        }
    }
}
