using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Toggl.Droid.Helper;
using static Toggl.Droid.Widgets.WidgetsConstants;

namespace Toggl.Droid.Widgets
{
    public abstract class TimeEntryWidgetStartStopButtonFormFactor : ITimeEntryWidgetFormFactor
    {
        public abstract RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo);

        protected void SetupActionsForStartAndStopButtons(Context context, RemoteViews remoteViews)
        {
            remoteViews.SetOnClickPendingIntent(Resource.Id.StartButton,
                StartTimeEntryAction.ToBroadcastIntent<TimeEntryWidget>(context));

            remoteViews.SetOnClickPendingIntent(Resource.Id.StopButton,
                StopRunningTimeEntryAction.ToBroadcastIntent<TimeEntryWidget>(context));
        }
    }
}
