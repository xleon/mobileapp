using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Fragments;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme",
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class MainActivity : ReactiveActivity<MainHostViewModel>
    {
        private MainFragment fragment => (MainFragment)SupportFragmentManager.Fragments.First();

        protected override void OnCreate(Bundle bundle)
        {
            var setup = MvxAndroidSetupSingleton.EnsureSingletonAvailable(ApplicationContext);
            setup.EnsureInitialized();

            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainActivity);
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out);
            InitializeViews();
        }

        public void SetupRatingViewVisibility(bool isVisible)
        {
            fragment?.SetupRatingViewVisibility(isVisible);
        }
    }
}
