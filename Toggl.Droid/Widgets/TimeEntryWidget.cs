using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Droid.Extensions;
using Toggl.Droid.Widgets.Services;
using Toggl.Shared;
using static Toggl.Droid.Services.TimerBackgroundService;
using static Toggl.Droid.Widgets.TimerWidgetFormFactor;
using static Toggl.Droid.Widgets.WidgetsConstants;
using Color = Android.Graphics.Color;

namespace Toggl.Droid.Widgets
{
    [BroadcastReceiver(Label = "Toggl Time Entry Widget", Exported = true)]
    [IntentFilter(new string[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData("android.appwidget.provider", Resource = "@xml/timeentrywidgetprovider")]
    public class TimeEntryWidget : AppWidgetProvider
    {
        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            switch (intent.Action)
            {
                case StartTimeEntryAction:
                    EnqueueWork(context, intent);
                    break;

                case StopRunningTimeEntryAction:
                    EnqueueWork(context, intent);
                    break;
            }
        }

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

            foreach (var widgetId in appWidgetIds)
            {
                updateWidget(context, appWidgetManager, widgetId);
            }
        }

        public override void OnAppWidgetOptionsChanged(Context context, AppWidgetManager appWidgetManager, int appWidgetId, Bundle newOptions)
        {
            base.OnAppWidgetOptionsChanged(context, appWidgetManager, appWidgetId, newOptions);
            var widgetContext = WidgetContext.From(newOptions);
            updateWidget(context, appWidgetManager, appWidgetId);

            var intent = new Intent(context, typeof(WidgetsAnalyticsService));
            intent.SetAction(WidgetsAnalyticsService.TimerWidgetResizeAction);
            intent.PutExtra(WidgetsAnalyticsService.TimerWidgetSizeParameter, widgetContext.ColumnsCount);
            WidgetsAnalyticsService.EnqueueWork(context, intent);
        }

        private void updateWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
        {
            var widgetOptions = appWidgetManager.GetAppWidgetOptions(appWidgetId);
            var widgetContext = WidgetContext.From(widgetOptions);
            var view = setupRemoteViews(context, widgetContext);

            var widgetInfo = TimeEntryWidgetInfo.FromSharedPreferences();

            if (widgetInfo.IsRunning)
            {
                view.SetViewVisibility(Resource.Id.StartButton, ViewStates.Gone);
                view.SetViewVisibility(Resource.Id.StopButton, ViewStates.Visible);

                var duration = (DateTimeOffset.Now - widgetInfo.StartTime).TotalMilliseconds;
                view.SetChronometerCountDown(Resource.Id.DurationTextView, false);
                view.SetChronometer(Resource.Id.DurationTextView, SystemClock.ElapsedRealtime() - (long)duration, "%s", true);

                if (string.IsNullOrEmpty(widgetInfo.Description))
                {
                    view.SetTextViewText(Resource.Id.DescriptionTextView, Resources.NoDescription);
                    view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.secondaryText));
                }
                else
                {
                    view.SetTextViewText(Resource.Id.DescriptionTextView, widgetInfo.Description);
                    view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.primaryText));
                }

                view.SetViewVisibility(Resource.Id.DotView, widgetInfo.HasProject.ToVisibility());
                view.SetViewVisibility(Resource.Id.ProjectTextView, widgetInfo.HasProject.ToVisibility());
                if (widgetInfo.HasProject)
                {
                    var projectColor = widgetInfo.ProjectColor != null ? Color.ParseColor(widgetInfo.ProjectColor) : Color.Black;
                    // Dot
                    view.SetInt(Resource.Id.DotView, "setBackgroundColor", projectColor);

                    // Project
                    view.SetTextViewText(Resource.Id.ProjectTextView, widgetInfo.ProjectName ?? "");
                    view.SetTextColor(Resource.Id.ProjectTextView, projectColor);

                    // Client
                    view.SetViewVisibility(Resource.Id.ClientTextView, widgetInfo.HasClient.ToVisibility());
                    if (widgetInfo.HasClient)
                        view.SetTextViewText(Resource.Id.ClientTextView, widgetInfo.ClientName);
                }
            }
            else
            {
                view.SetViewVisibility(Resource.Id.StartButton, ViewStates.Visible);
                view.SetViewVisibility(Resource.Id.StopButton, ViewStates.Gone);

                view.SetChronometerCountDown(Resource.Id.DurationTextView, false);
                view.SetChronometer(Resource.Id.DurationTextView, SystemClock.ElapsedRealtime(), "%s", false);

                view.SetTextViewText(Resource.Id.DescriptionTextView, Resources.NoDescription);
                view.SetTextColor(Resource.Id.DescriptionTextView, context.SafeGetColor(Resource.Color.secondaryText));

                view.SetViewVisibility(Resource.Id.DotView, false.ToVisibility());
                view.SetViewVisibility(Resource.Id.ProjectTextView, false.ToVisibility());
                view.SetViewVisibility(Resource.Id.ClientTextView, false.ToVisibility());
            }

            appWidgetManager.UpdateAppWidget(appWidgetId, view);
        }

        private void reportInstallationState(Context context, bool installed)
        {
            var intent = new Intent(context, typeof(WidgetsAnalyticsService));
            intent.SetAction(WidgetsAnalyticsService.TimerWidgetInstallAction);
            intent.PutExtra(WidgetsAnalyticsService.TimerWidgetInstallStateParameter, installed);
            WidgetsAnalyticsService.EnqueueWork(context, intent);
        }

        private RemoteViews setupRemoteViews(Context context, WidgetContext widgetContext)
        {
            switch (widgetContext.FormFactor)
            {
                case ButtonOnly:
                    return setupButtonOnlyRemoteViews(context);

                case FullWidget:
                    return setupFullWidgetRemoteViews(context);

                default:
                    throw new InvalidOperationException("Invalid form factor.");
            }
        }

        private RemoteViews setupButtonOnlyRemoteViews(Context context)
        {
            var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidgetSmall);
            remoteViews.SetOnClickPendingIntent(Resource.Id.StartButton, getStartTimeEntryPendingIntent(context));
            remoteViews.SetOnClickPendingIntent(Resource.Id.StopButton, getStopTimeEntryPendingIntent(context));

            return remoteViews;
        }

        private RemoteViews setupFullWidgetRemoteViews(Context context)
        {
            var remoteViews = new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidget);
            remoteViews.SetOnClickPendingIntent(Resource.Id.StartButton, getStartTimeEntryPendingIntent(context));
            remoteViews.SetOnClickPendingIntent(Resource.Id.StopButton, getStopTimeEntryPendingIntent(context));
            return remoteViews;
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
