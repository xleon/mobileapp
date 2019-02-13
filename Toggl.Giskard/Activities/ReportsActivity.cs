using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Fragments;

namespace Toggl.Giskard.Activities
{
    [MvxActivityPresentation]
    [Activity(Theme = "@style/AppTheme",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public sealed partial class ReportsActivity : ReactiveActivity<ReportsHostViewModel>
    {
        private ReportsFragment fragment => (ReportsFragment)SupportFragmentManager.Fragments.First();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReportsActivity);
            OverridePendingTransition(Resource.Animation.abc_slide_in_right, Resource.Animation.abc_fade_out);
            InitializeViews();
        }

        public override void OnEnterAnimationComplete()
        {
            base.OnEnterAnimationComplete();
            fragment?.ViewModel.StopNavigationFromMainLogStopwatch();
        }

        public override void Finish()
        {
            base.Finish();
            OverridePendingTransition(Resource.Animation.abc_fade_in, Resource.Animation.abc_slide_out_right);
        }

        internal void ToggleCalendarState(bool forceHide)
        {
            fragment?.ToggleCalendarState(forceHide);
        }
    }
}
