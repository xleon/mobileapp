using System.Collections.Generic;
using System.Threading;
using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Util;
using Toggl.Core.Analytics;
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

        private void reportInstallationState(Context context, bool installed)
        {
            var intent = new Intent(context, typeof(InstallationStateReportService));
            intent.PutExtra(InstallationStateReportService.StateParameterName, installed);
            InstallationStateReportService.EnqueueWork(context, intent);
        }
    }
}
