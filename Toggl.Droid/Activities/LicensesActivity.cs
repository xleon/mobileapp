using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.ViewHolders;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using System.Linq;
using CoreResource = Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class LicensesActivity : ReactiveActivity<LicensesViewModel>
    {
        private RecyclerView recyclerView;

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppTheme_Light);
            base.OnCreate(bundle);
            if (ViewModelWasNotCached())
            {
                BailOutToSplashScreen();
                return;
            }
            SetContentView(Resource.Layout.LicensesActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out);

            InitializeViews();

            var adapter = new SimpleAdapter<License>(Resource.Layout.LicensesActivityCell, LicenseViewHolder.Create);
            var layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.SetAdapter(adapter);

            adapter.Items = ViewModel.Licenses.ToList();

            setupToolbar();
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right);
        }

        private void setupToolbar()
        {
            var toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.Title = CoreResource.Licenses;

            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            toolbar.Rx()
                .NavigationTapped()
                .Subscribe(onNavigateBack)
                .DisposedBy(DisposeBag);
        }

        private void onNavigateBack()
        {
            ViewModel.CloseWithDefaultResult();
        }

        protected override void InitializeViews()
        {
            recyclerView = FindViewById<RecyclerView>(Resource.Id.RecyclerView);
        }
    }
}
