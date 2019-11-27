using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Calendar;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Droid.Adapters.Calendar;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Droid.Views.Calendar;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public partial class CalendarFragment : ReactiveTabFragment<CalendarViewModel>, IScrollableToTop
    {
        private CalendarLayoutManager calendarLayoutManager;

        public CalendarFragment(MainTabBarViewModel tabBarViewModel)
            : base(tabBarViewModel)
        {
        }

        public CalendarFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializationFinished()
        {
            var timeService = AndroidDependencyContainer.Instance.TimeService;
            var schedulerProvider = AndroidDependencyContainer.Instance.SchedulerProvider;

            calendarLayoutManager = new CalendarLayoutManager();
            calendarRecyclerView.SetLayoutManager(calendarLayoutManager);
            var displayMetrics = new DisplayMetrics();
            Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
            var calendarAdapter = new CalendarAdapter(Context, timeService, displayMetrics.WidthPixels);
            calendarRecyclerView.SetTimeService(timeService);
            calendarRecyclerView.SetAdapter(calendarAdapter);

            timeService
                .CurrentDateTimeObservable
                .DistinctUntilChanged(offset => offset.Day)
                .ObserveOn(schedulerProvider.MainScheduler)
                .Subscribe(configureHeaderDate)
                .DisposedBy(DisposeBag);

            ViewModel.HasCalendarsLinked.SelectUnit()
                .Merge(ViewModel.CalendarItems.CollectionChange.SelectUnit())
                .SelectMany(ViewModel.HasCalendarsLinked)
                .Subscribe(hasCalendarsLinked =>
                {
                    calendarAdapter.UpdateItems(ViewModel.CalendarItems, hasCalendarsLinked);
                    calendarRecyclerView.SetHasTwoColumns(hasCalendarsLinked);
                })
                .DisposedBy(DisposeBag);

            ViewModel.HasCalendarsLinked
                .Subscribe(headerCalendarEventsView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CalendarItems.CollectionChange
                .SelectUnit()
                .StartWith(Unit.Default)
                .Select(_ => calculateCalendarEventsCount())
                .ObserveOn(schedulerProvider.MainScheduler)
                .Subscribe(updateCalendarEventsCount)
                .DisposedBy(DisposeBag);

            ViewModel.TimeTrackedToday
                .Subscribe(headerTimeEntriesTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.HasCalendarsLinked
                .Invert()
                .Subscribe(headerLinkCalendarsButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            headerLinkCalendarsButton.Rx().Tap()
                .Subscribe(ViewModel.LinkCalendars.Inputs)
                .DisposedBy(DisposeBag);

            calendarAdapter.CalendarItemTappedObservable
                .Subscribe(ViewModel.OnItemTapped.Inputs)
                .DisposedBy(DisposeBag);

            calendarRecyclerView.EmptySpansTouchedObservable
                .Where(_ => !calendarAdapter.NeedsToClearItemInEditMode())
                .Subscribe(ViewModel.CreateTimeEntryAtOffset.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.ShouldShowOnboarding
                .Subscribe(onboardingVisibilityChanged)
                .DisposedBy(DisposeBag);
        }

        public void ScrollToTop()
        {
            calendarRecyclerView?.SmoothScrollToPosition(0);
        }

        private void onboardingVisibilityChanged(bool visible)
        {
            if (visible)
            {
                initializeOnboardingViewIfNeeded();

                appBarLayout.Visibility = ViewStates.Gone;
                onboardingView.Visibility = ViewStates.Visible;
                calendarRecyclerView.Visibility = ViewStates.Gone;
                return;
            }

            appBarLayout.Visibility = ViewStates.Visible;
            calendarRecyclerView.Visibility = ViewStates.Visible;

            if (onboardingView == null)
                return;

            onboardingView.Visibility = ViewStates.Gone;
        }

        private void initializeOnboardingViewIfNeeded()
        {
            if (onboardingView != null)
                return;

            onboardingView = onboardingViewStub.Inflate();
            onboardingTitleView = onboardingView.FindViewById<TextView>(Resource.Id.CalendarOnboardingTitle);
            onboardingMessageView = onboardingView.FindViewById<TextView>(Resource.Id.CalendarOnboardingMessage);
            getStartedButton = onboardingView.FindViewById<Button>(Resource.Id.CalendarOnboardingGetStartedButton);
            skipButton = onboardingView.FindViewById<TextView>(Resource.Id.CalendarOnboardingSkipButton);

            onboardingTitleView.Text = Shared.Resources.CalendarOnboardingTitle;
            onboardingMessageView.Text = Shared.Resources.CalendarOnboardingMessage;
            getStartedButton.Text = Shared.Resources.LinkYourCalendars;
            skipButton.Text = Shared.Resources.Skip;

            getStartedButton.Rx().Tap()
                .Subscribe(ViewModel.GetStarted.Inputs)
                .DisposedBy(DisposeBag);

            skipButton.Rx().Tap()
                .Subscribe(ViewModel.SkipOnboarding.Inputs)
                .DisposedBy(DisposeBag);
        }

        private void updateCalendarEventsCount(int count)
        {
            var text = string.Format(Shared.Resources.TotalEvents, count.ToString());
            headerCalendarEventsTextView.Text = text;
        }

        private int calculateCalendarEventsCount()
        {
            return ViewModel
                .CalendarItems
                .SelectMany(group => group.Where(item => item.Source == CalendarItemSource.Calendar))
                .Count();
        }

        private void configureHeaderDate(DateTimeOffset offset)
        {
            var day = offset.Day.ToString();
            headerDayTextView.Text = day;
            headerWeekdayTextView.Text = offset.ToString("ddd");
        }
    }
}
