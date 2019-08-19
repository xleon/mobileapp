using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Droid.Fragments
{
    public partial class SelectBeginningOfWeekFragment
    {
        private TextView title;
        private RecyclerView recyclerView;

        protected override void InitializeViews(View view)
        {
            title = view.FindViewById<TextView>(Resource.Id.TitleView);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.DaysListRecyclerView);
        }
    }
}
