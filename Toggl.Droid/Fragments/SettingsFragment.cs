using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using static Toggl.Shared.Resources;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SettingsFragment : ReactiveTabFragment<SettingsViewModel>, IScrollableToTop
    {
        public SettingsFragment(MainTabBarViewModel tabBarViewModel)
            : base(tabBarViewModel)
        {
        }

        public SettingsFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializationFinished()
        {
            scrollView.AttachMaterialScrollBehaviour(appBarLayout);
            SetupToolbar(View, title: Settings);

            versionTextView.Text = ViewModel.Version;

            ViewModel.Name
                .Subscribe(nameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Email
                .Subscribe(emailTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(defaultWorkspaceNameTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.SwipeActionsEnabled
                .Subscribe(swipeActionsSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsManualModeEnabled
                .Subscribe(manualModeSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsGroupingTimeEntries
               .Subscribe(groupTimeEntriesSwitch.Rx().CheckedObserver())
               .DisposedBy(DisposeBag);

            ViewModel.UseTwentyFourHourFormat
                .Subscribe(is24hoursModeSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.AreRunningTimerNotificationsEnabled
                .Subscribe(runningTimerNotificationsSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.AreStoppedTimerNotificationsEnabled
                .Subscribe(stoppedTimerNotificationsSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.DateFormat
                .Subscribe(dateFormatTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.BeginningOfWeek
                .Subscribe(beginningOfWeekTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.DurationFormat
                .Subscribe(durationFormatTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsCalendarSmartRemindersVisible
                .Subscribe(smartRemindersView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsCalendarSmartRemindersVisible
                .Subscribe(smartRemindersViewSeparator.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.CalendarSmartReminders
                .Subscribe(smartRemindersTextView.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.LoggingOut
                .Subscribe(Context.CancelAllNotifications)
                .DisposedBy(DisposeBag);

            ViewModel.IsFeedbackSuccessViewShowing
                .Subscribe(showFeedbackSuccessToast)
                .DisposedBy(DisposeBag);

            logoutView.Rx()
                .BindAction(ViewModel.TryLogout)
                .DisposedBy(DisposeBag);

            helpView.Rx()
                .BindAction(ViewModel.OpenHelpView)
                .DisposedBy(DisposeBag);

            aboutView.Rx()
                .BindAction(ViewModel.OpenAboutView)
                .DisposedBy(DisposeBag);

            defaultWorkspaceView.Rx()
                .BindAction(ViewModel.PickDefaultWorkspace)
                .DisposedBy(DisposeBag);

            feedbackView.Rx()
                .BindAction(ViewModel.SubmitFeedback)
                .DisposedBy(DisposeBag);

            swipeActionsView.Rx()
                .BindAction(ViewModel.ToggleSwipeActions)
                .DisposedBy(DisposeBag);

            manualModeView.Rx()
                .BindAction(ViewModel.ToggleManualMode)
                .DisposedBy(DisposeBag);

            groupTimeEntriesView.Rx()
                .BindAction(ViewModel.ToggleTimeEntriesGrouping)
                .DisposedBy(DisposeBag);

            is24hoursModeView.Rx()
                .BindAction(ViewModel.ToggleTwentyFourHourSettings)
                .DisposedBy(DisposeBag);

            runningTimerNotificationsView.Rx().Tap()
                .Subscribe(ViewModel.ToggleRunningTimerNotifications)
                .DisposedBy(DisposeBag);

            stoppedTimerNotificationsView.Rx().Tap()
                .Subscribe(ViewModel.ToggleStoppedTimerNotifications)
                .DisposedBy(DisposeBag);

            dateFormatView.Rx().Tap()
                .Subscribe(ViewModel.SelectDateFormat.Inputs)
                .DisposedBy(DisposeBag);

            beginningOfWeekView.Rx()
                .BindAction(ViewModel.SelectBeginningOfWeek)
                .DisposedBy(DisposeBag);

            durationFormatView.Rx().Tap()
                .Subscribe(ViewModel.SelectDurationFormat.Inputs)
                .DisposedBy(DisposeBag);

            calendarSettingsView.Rx().Tap()
                .Subscribe(ViewModel.OpenCalendarSettings.Inputs)
                .DisposedBy(DisposeBag);

            smartRemindersView.Rx().Tap()
                .Subscribe(ViewModel.OpenCalendarSmartReminders.Inputs)
                .DisposedBy(DisposeBag);
        }

        public void ScrollToTop()
        {
            scrollView?.SmoothScrollTo(0, 0);
        }

        private void showFeedbackSuccessToast(bool succeeeded)
        {
            if (!succeeeded) return;

            var toast = Toast.MakeText(Context, Shared.Resources.SendFeedbackSuccessMessage, ToastLength.Long);
            toast.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom, 0, 0);
            toast.Show();
        }
    }
}
