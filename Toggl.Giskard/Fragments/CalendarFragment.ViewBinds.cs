using Android.Views;
using Toggl.Giskard.Views;
using Android.Widget;

namespace Toggl.Giskard.Fragments
{
    public partial class CalendarFragment
    {
        private CalendarRecyclerView calendarRecyclerView;
        private ViewStub onboardingViewStub;
        private View onboardingView;
        private Button getStartedButton;
        private TextView skipButton;

        protected override void InitializeViews(View view)
        {
            calendarRecyclerView = view.FindViewById<CalendarRecyclerView>(Resource.Id.calendarRecyclerView);
            onboardingViewStub = view.FindViewById<ViewStub>(Resource.Id.OnboardingViewStub);
        }
    }
}
