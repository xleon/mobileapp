using Android.Widget;

namespace Toggl.Droid.Activities
{
    public sealed partial class OutdatedAppActivity
    {
        private Button updateAppButton;
        private Button openWebsiteButton;

        protected override void InitializeViews()
        {
            updateAppButton = FindViewById<Button>(Resource.Id.UpdateAppButton);
            openWebsiteButton = FindViewById<Button>(Resource.Id.OpenWebsiteButton);
        }
    }
}
