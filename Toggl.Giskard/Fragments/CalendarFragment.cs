using System;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Giskard.Adapters.Calendar;
using Toggl.Giskard.Views.Calendar;
using Toggl.Giskard.Extensions.Reactive;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Fragments
{
    public partial class CalendarFragment : ReactiveFragment<CalendarViewModel>
    {
        private CalendarLayoutManager calendarLayoutManager;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CalendarFragment, container, false);
            InitializeViews(view);

            calendarLayoutManager = new CalendarLayoutManager();
            calendarRecyclerView.SetLayoutManager(calendarLayoutManager);
            var displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            calendarRecyclerView.SetAdapter(new CalendarAdapter(view.Context, displayMetrics.WidthPixels));

            ViewModel.ShouldShowOnboarding
                .Subscribe(onboardingVisibilityChanged)
                .DisposedBy(DisposeBag);

            return view;
        }

        private void onboardingVisibilityChanged(bool visible)
        {
            if (visible)
            {
                if (onboardingView == null)
                {
                    initializeOnboardingView();
                }
                onboardingView.Visibility = ViewStates.Visible;
            }
            else if (onboardingView != null)
            {
                onboardingView.Visibility = ViewStates.Gone;
            }
        }

        private void initializeOnboardingView()
        {
            onboardingView = onboardingViewStub.Inflate();
            getStartedButton = onboardingView.FindViewById<Button>(Resource.Id.CalendarOnboardingGetStartedButton);
            skipButton = onboardingView.FindViewById<TextView>(Resource.Id.CalendarOnboardingSkipButton);

            getStartedButton.Rx().Tap()
                .Subscribe(ViewModel.GetStarted.Inputs)
                .DisposedBy(DisposeBag);

            skipButton.Rx().Tap()
                .Subscribe(ViewModel.SkipOnboarding.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
