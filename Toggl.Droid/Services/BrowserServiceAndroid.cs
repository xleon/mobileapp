using Android.Content;
using Android.Net;
using MvvmCross.Platforms.Android;
using Toggl.Core.UI.Services;

namespace Toggl.Droid.Services
{
    public sealed class BrowserServiceAndroid : MvxAndroidTask, IBrowserService
    {
        public void OpenStore()
        {
            OpenUrl("https://play.google.com/store/apps/details?id=com.toggl.giskard");
        }

        public void OpenUrl(string url)
        {
            var intent = new Intent(Intent.ActionView).SetData(Uri.Parse(url));
            StartActivity(intent);
        }
    }
}
