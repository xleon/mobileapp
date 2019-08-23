using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;

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
            
            title.Text = Shared.Resources.FirstDayOfTheWeek;
            adapter = new SimpleAdapter<SelectableBeginningOfWeekViewModel>(
                Resource.Layout.SelectBeginningOfWeekFragmentCell,
                BeginningOfWeekViewHolder.Create);
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));
            recyclerView.SetAdapter(adapter);
        }
    }
}
