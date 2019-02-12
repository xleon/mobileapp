using Android.Widget;

namespace Toggl.Giskard.Activities
{
    public sealed partial class ReportsActivity
    {
        private FrameLayout reportsFragmentContainer;

        protected override void InitializeViews()
        {
            reportsFragmentContainer = FindViewById<FrameLayout>(Resource.Id.ReportsFragmentContainer);
        }
    }
}
