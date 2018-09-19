using Android.Support.Design.Widget;
using Android.Widget;

namespace Toggl.Giskard.Activities
{
    public sealed partial class ReportsActivity
    {
        private FloatingActionButton selectWorkspaceFAB;
        private TextView workspaceName;

        private void initializeViews()
        {
            selectWorkspaceFAB = FindViewById<FloatingActionButton>(Resource.Id.SelectWorkspaceFAB);
            workspaceName = FindViewById<TextView>(Resource.Id.ReportsActivityWorkspaceName);
        }
    }
}
