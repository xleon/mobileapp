using Android.Support.V7.Widget;
using Android.Views;
using Toggl.Droid.Adapters;

namespace Toggl.Droid.Fragments
{
    public partial class SelectDurationFormatFragment
    {
        private RecyclerView recyclerView;
        private SelectDurationFormatRecyclerAdapter selectDurationRecyclerAdapter;

        private void initializeViews(View view)
        {
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.SelectDurationFormatRecyclerView);
        }
    }
}
