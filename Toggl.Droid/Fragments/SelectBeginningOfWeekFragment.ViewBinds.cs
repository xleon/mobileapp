using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Views;

namespace Toggl.Droid.Fragments
{
    public partial class SelectBeginningOfWeekFragment
    {
        private RecyclerView recyclerView;
        
        private void initializeViews(View view)
        {
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.DaysListRecyclerView);
        }
    }
}
