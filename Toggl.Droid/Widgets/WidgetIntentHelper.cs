using Android.App;
using Android.Content;

namespace Toggl.Droid.Widgets
{
    public static class WidgetIntentHelper
    {
        public static PendingIntent GetOpenAppPendingIntent(Context context)
        {
            var intent = context.PackageManager.GetLaunchIntentForPackage(context.PackageName);
            intent.SetPackage(null);
            intent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded | ActivityFlags.TaskOnHome);
            return PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent);
        }
    }
}
