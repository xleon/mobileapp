using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Fragments
{
    public partial class SelectDurationFormatFragment
    {
        private TextView titleLabel;
        private RecyclerView recyclerView;
        private SelectDurationFormatRecyclerAdapter selectDurationRecyclerAdapter;

        protected override void InitializeViews(View view)
        {
            titleLabel = view.FindViewById<TextView>(Resource.Id.SelectDurationTitle);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.SelectDurationFormatRecyclerView);
        }
    }
}
