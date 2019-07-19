using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Widget;
using Toggl.Droid.Widgets.Services;

namespace Toggl.Droid.Widgets
{
    [BroadcastReceiver(Label = "Toggl Time Entry Widget", Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/timeentrywidgetprovider")]
    public class TimeEntryWidget : AppWidgetProvider
    {
        public override void OnDeleted(Context context, int[] appWidgetIds)
        {
            reportInstallationState(context, false);
            base.OnDeleted(context, appWidgetIds);
        }

        public override void OnEnabled(Context context)
        {
            reportInstallationState(context, true);
            base.OnEnabled(context);
        }

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
            var minWidth = newOptions.GetInt(AppWidgetManager.OptionAppwidgetMinWidth);
            appWidgetManager.UpdateAppWidget(appWidgetId, getRemoteViews(context, minWidth));
        }

        private void reportInstallationState(Context context, bool installed)
        {
            var intent = new Intent(context, typeof(InstallationStateReportService));
            intent.PutExtra(InstallationStateReportService.StateParameterName, installed);
            InstallationStateReportService.EnqueueWork(context, intent);
        }

        private RemoteViews getRemoteViews(Context context, int minWidth)
            => minWidth < 110
                ? new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidgetSmall)
                : new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidget);
    }
}
