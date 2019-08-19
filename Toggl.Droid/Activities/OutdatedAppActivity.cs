using Android.App;
using Android.Content.PM;
using Android.OS;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class OutdatedAppActivity : ReactiveActivity<OutdatedAppViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppTheme_Light_OutdatedAppStatusBarColor);
            base.OnCreate(bundle);
            if (ViewModelWasNotCached())
            {
                BailOutToSplashScreen();
                return;
            }
            SetContentView(Resource.Layout.OutdatedAppActivity);
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
            InitializeViews();

            oopsTextView.Text = Shared.Resources.Oops;
            outdatedAppMessageView.Text = Shared.Resources.AppOutdatedMessage;
            openWebsiteButton.Text = Shared.Resources.OutdatedAppTryTogglCom;
            updateAppButton.Text = Shared.Resources.UpdateTheApp;

            updateAppButton.Rx()
                .BindAction(ViewModel.UpdateApp)
                .DisposedBy(DisposeBag);

            openWebsiteButton.Rx()
                .BindAction(ViewModel.OpenWebsite)
                .DisposedBy(DisposeBag);
        }
    }
}
