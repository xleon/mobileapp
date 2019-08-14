using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class AboutActivity : ReactiveActivity<AboutViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.AppTheme_Light);
            base.OnCreate(bundle);
            if (ViewModelWasNotCached())
            {
                BailOutToSplashScreen();
                return;
            }
            SetContentView(Resource.Layout.AboutActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out);

            InitializeViews();

            licensesButton.Rx()
                .BindAction(ViewModel.OpenLicensesView)
                .DisposedBy(DisposeBag);

            privacyPolicyButton.Rx()
                .BindAction(ViewModel.OpenPrivacyPolicyView)
                .DisposedBy(DisposeBag);

            termsOfServiceButton.Rx()
                .BindAction(ViewModel.OpenTermsOfServiceView)
                .DisposedBy(DisposeBag);

            SetupToolbar(title: GetString(Resource.String.About));
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right);
        }
    }
}
