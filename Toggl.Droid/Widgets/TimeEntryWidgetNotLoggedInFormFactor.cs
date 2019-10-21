using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Toggl.Shared;
using static Toggl.Droid.Widgets.WidgetsConstants;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Widgets
{
    public sealed class TimeEntryWidgetNotLoggedInFormFactor : ITimeEntryWidgetFormFactor
    {
        private bool isFullView;

        public TimeEntryWidgetNotLoggedInFormFactor(int columnCount)
        {
            isFullView = columnCount > 3;
        }

        public RemoteViews Setup(Context context, TimeEntryWidgetInfo widgetInfo)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.TimeEntryWidgetNotLoggedIn);

            view.SetOnClickPendingIntent(Resource.Id.RootLayout, getOpenAppToLoginPendingIntent(context));

            view.SetTextViewText(Resource.Id.NotLoggedInLabel, Resources.LoginToTrack);

            view.SetViewVisibility(Resource.Id.NotLoggedInLabel, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.TogglLogo, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.PadLock, isFullView.ToVisibility());
            view.SetViewVisibility(Resource.Id.PadLockBig, (!isFullView).ToVisibility());

            return view;
        }

        private PendingIntent getOpenAppToLoginPendingIntent(Context context)
        {
            var intent = new Intent(context, typeof(SplashScreen)).AddFlags(ActivityFlags.TaskOnHome);
            return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}
