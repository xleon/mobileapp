using Android.App;
using Android.Content.PM;
using Android.OS;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/AppTheme.OutdatedAppStatusBarColor",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class OutdatedAppActivity : ReactiveActivity<OutdatedAppViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.OutdatedAppActivity);
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
            InitializeViews();

            updateAppButton.Rx()
                .BindAction(ViewModel.UpdateApp)
                .DisposedBy(DisposeBag);

            openWebsiteButton.Rx()
                .BindAction(ViewModel.OpenWebsite)
                .DisposedBy(DisposeBag);
        }
    }
}
