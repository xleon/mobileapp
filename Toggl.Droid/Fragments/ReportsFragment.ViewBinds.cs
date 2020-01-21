using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Toggl.Droid.Extensions;
using Toggl.Droid.LayoutManagers;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment
    {
        private readonly ReportsAdapter adapter = new ReportsAdapter();

        protected override int LayoutId => Resource.Layout.ReportsFragment;

        protected override View LoadingPlaceholderView { get; set; }

        private FloatingActionButton selectWorkspaceFab;
        private TextView toolbarCurrentDateRangeText;
        private RecyclerView reportsRecyclerView;
        private AppBarLayout appBarLayout;

        protected override void InitializeViews(View fragmentView)
        {
            selectWorkspaceFab = fragmentView.FindViewById<FloatingActionButton>(Resource.Id.SelectWorkspaceFAB);
            toolbarCurrentDateRangeText = fragmentView.FindViewById<TextView>(Resource.Id.ToolbarCurrentDateRangeText);
            reportsRecyclerView = fragmentView.FindViewById<RecyclerView>(Resource.Id.ReportsFragmentRecyclerView);
            appBarLayout = fragmentView.FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);

            LoadingPlaceholderView = fragmentView.FindViewById(Resource.Id.TabLoadingIndicator);

            setupRecyclerView();
        }

        private void setupRecyclerView()
        {
            reportsRecyclerView.AttachMaterialScrollBehaviour(appBarLayout);
            reportsRecyclerView.SetLayoutManager(new UnpredictiveLinearLayoutManager(Context));
            reportsRecyclerView.SetAdapter(adapter);
        }
    }
}
