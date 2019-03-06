using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Threading;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using MvvmCross;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Giskard.Adapters.Calendar;
using Toggl.Giskard.Views.Calendar;
using Toggl.Giskard.Extensions.Reactive;
using Toggl.Giskard.Views;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Fragments
{
    public partial class CalendarFragment : ReactiveFragment<CalendarViewModel>
    {
        private CalendarLayoutManager calendarLayoutManager;
        private ITimeService timeService;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.CalendarFragment, container, false);
            InitializeViews(view);

            timeService = Mvx.Resolve<ITimeService>();
            calendarLayoutManager = new CalendarLayoutManager();
            calendarRecyclerView.SetLayoutManager(calendarLayoutManager);
            var displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            var calendarAdapter = new CalendarAdapter(view.Context, timeService,displayMetrics.WidthPixels);
            calendarRecyclerView.SetTimeService(timeService);
            calendarRecyclerView.SetAdapter(calendarAdapter);

            ViewModel.HasCalendarsLinked.SelectUnit()
                .Merge(ViewModel.CalendarItems.CollectionChange.SelectUnit())
                .SelectMany(ViewModel.HasCalendarsLinked)
                .Subscribe(hasCalendarsLinked =>
                {
                    calendarAdapter.UpdateItems(ViewModel.CalendarItems, hasCalendarsLinked);
                    calendarRecyclerView.SetHasTwoColumns(hasCalendarsLinked);
                })
                .DisposedBy(DisposeBag);

            calendarAdapter.CalendarItemTappedObservable
                .Subscribe(ViewModel.OnItemTapped.Inputs)
                .DisposedBy(DisposeBag);

            calendarRecyclerView.EmptySpansTouchedObservable
                .Where(_ => !calendarAdapter.NeedsToClearItemInEditMode())
                .Select(span => (span, TimeSpan.FromMinutes(30)))
                .Subscribe(ViewModel.OnDurationSelected.Inputs)
                .DisposedBy(DisposeBag);

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
