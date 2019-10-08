using Android.App;
using Android.Appwidget;
using Android.Content;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Services;
using Toggl.Droid.Extensions;
using Toggl.Droid.Widgets;

namespace Toggl.Droid.Services
{
    public sealed class TimerWidgetServiceAndroid : TimerWidgetService
    {
        public TimerWidgetServiceAndroid(ITogglDataSource dataSource) : base(dataSource)
        {
        }

        protected override void OnRunningTimeEntryChanged(IThreadSafeTimeEntry timeEntry)
        {
            TimeEntryWidgetInfo.Save(timeEntry);

            var context = Application.Context;
            var widgetClass = JavaUtils.ToClass<TimeEntryWidget>();

            var widgetIds = AppWidgetManager
                .GetInstance(context)
                .GetAppWidgetIds(new ComponentName(context, widgetClass));

            var intent = new Intent(Application.Context, widgetClass);
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, widgetIds);

            context.SendBroadcast(intent);
        }
    }
}
