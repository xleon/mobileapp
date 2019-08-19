using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SelectDateFormatFragment
    {
        private TextView titleLabel;
        private RecyclerView recyclerView;

        protected override void InitializeViews(View rootView)
        {
            titleLabel = rootView.FindViewById<TextView>(Resource.Id.SelectDateFormatTitle);
            recyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.SelectDateFormatRecyclerView);
        }
    }
}
