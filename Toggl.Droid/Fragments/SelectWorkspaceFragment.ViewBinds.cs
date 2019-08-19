using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public partial class SelectWorkspaceFragment
    {
        private TextView titleLabel;
        private RecyclerView recyclerView;

        protected override void InitializeViews(View rootView)
        {
            titleLabel = rootView.FindViewById<TextView>(Resource.Id.SelectWorkspaceTitle);
            recyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.SelectWorkspaceRecyclerView);
        }
    }
}
