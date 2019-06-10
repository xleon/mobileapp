using Android.Support.V7.Widget;
using Android.Views;

namespace Toggl.Droid.Fragments
{
    public partial class SelectBeginningOfWeekFragment
    {
        private RecyclerView recyclerView;
        
        protected override void InitializeViews(View view)
        {
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.DaysListRecyclerView);
        }
    }
}
