using System;
using Android.App;
using Android.Content;
using Android.Widget;
using static Toggl.Droid.Widgets.WidgetsConstants;

namespace Toggl.Droid.Widgets
{
    public abstract class TimeEntryWidgetStartStopButtonFormFactor : ITimeEntryWidgetFormFactor
    {
        public abstract RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo);

        protected void SetupActionsForStartAndStopButtons(Context context, RemoteViews remoteViews)
        {
            remoteViews.SetOnClickPendingIntent(Resource.Id.StartButton, getStartTimeEntryPendingIntent(context));
            remoteViews.SetOnClickPendingIntent(Resource.Id.StopButton, getStopTimeEntryPendingIntent(context));
        }

        private PendingIntent getStartTimeEntryPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(TimeEntryWidget));
            intent.SetAction(StartTimeEntryAction);
            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }

        private PendingIntent getStopTimeEntryPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(TimeEntryWidget));
            intent.SetAction(StopRunningTimeEntryAction);
            return PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}
