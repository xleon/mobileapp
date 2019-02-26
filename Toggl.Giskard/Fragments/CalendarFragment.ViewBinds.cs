using Android.Views;
using Android.Widget;

namespace Toggl.Giskard.Fragments
{
    public partial class CalendarFragment
    {
        private ViewStub onboardingViewStub;
        private View onboardingView;

        private Button getStartedButton;
        private TextView skipButton;

        protected override void InitializeViews(View view)
        {
            onboardingViewStub = view.FindViewById<ViewStub>(Resource.Id.OnboardingViewStub);
        }
    }
}
