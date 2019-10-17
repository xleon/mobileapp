using Android.App;
using Android.Appwidget;
using Android.Content;
using System;
using System.Linq;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Droid.Extensions;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Droid.Extensions
{
    public static class WidgetsExtensions
    {
        public static void RequestActionFromWidget<TWidget>(this Context context, string action)
        {
            var widgetClass = JavaUtils.ToClass<TWidget>();

            var widgetIds = AppWidgetManager
                .GetInstance(context)
                .GetAppWidgetIds(new ComponentName(context, widgetClass));

            var intent = new Intent(Application.Context, widgetClass);
            intent.SetAction(action);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, widgetIds);

            context.SendBroadcast(intent);
        }
    }
}
