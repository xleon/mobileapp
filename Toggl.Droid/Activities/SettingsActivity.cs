using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Toggl.Core.Sync;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;
using Toggl.Core.UI.Views.Settings;
using Toggl.Droid.ViewHolders.Settings;
using static Toggl.Shared.Resources;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SettingsActivity : ReactiveActivity<SettingsViewModel>
    {
        public SettingsActivity() : base(
            Resource.Layout.SettingsActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromBottom)
        {
        }

        public SettingsActivity(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            scrollView.AttachMaterialScrollBehaviour(appBarLayout);

            // profile section
            ViewModel.Name
                .Select(name => new InfoRow(Name, name))
                .Subscribe(nameRow.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.Email
                .Select(email => new InfoRow(Email, email))
                .Subscribe(emailRow.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.WorkspaceName
                .Select(workspaceName => new NavigationRow(Workspace, workspaceName, ViewModel.PickDefaultWorkspace))
                .Subscribe(workspaceRow.SetRowData)
                .DisposedBy(DisposeBag);

            // date and time section
            ViewModel.DateFormat
                .Select(dateFormat => new NavigationRow(DateFormat, dateFormat, ViewModel.SelectDateFormat))
                .Subscribe(dateFormatRow.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.UseTwentyFourHourFormat
            .Select(use24HourFormat => new ToggleRow(Use24HourClock, use24HourFormat, ViewModel.ToggleTwentyFourHourSettings))
            .Subscribe(use24HoursFormatRow.SetRowData)
            .DisposedBy(DisposeBag);
              
            ViewModel.DurationFormat
            .Select(durationFormat => new NavigationRow(DurationFormat, durationFormat, ViewModel.SelectDurationFormat))
            .Subscribe(durationFormatRow.SetRowData)
            .DisposedBy(DisposeBag);
              
            ViewModel.BeginningOfWeek
            .Select(beginningOfWeek => new NavigationRow(FirstDayOfTheWeek, beginningOfWeek, ViewModel.SelectBeginningOfWeek))
            .Subscribe(beginningOfWeekRow.SetRowData)
            .DisposedBy(DisposeBag);
              
            ViewModel.IsGroupingTimeEntries
            .Select(groupTEs => new ToggleRow(GroupTimeEntries, groupTEs, ViewModel.ToggleTimeEntriesGrouping))
            .Subscribe(isGroupingTimeEntriesRow.SetRowData)
            .DisposedBy(DisposeBag);

            
            // timer defaults section 
            ViewModel.IsManualModeEnabled
                .Select(isManualModeEnabled => new ToggleRowWithDescription(ManualMode, ManualModeDescription, isManualModeEnabled, ViewModel.ToggleManualMode))
                .Subscribe(isManualModeEnabledRowView.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.SwipeActionsEnabled
                .Select(areSwipeActionsEnabled => new ToggleRow(SwipeActions, areSwipeActionsEnabled, ViewModel.ToggleSwipeActions))
                .Subscribe(swipeActionsRow.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.AreRunningTimerNotificationsEnabled
                .Select(runningTimerNotificationsEnabled => new ToggleRow(NotificationsRunningTimer, runningTimerNotificationsEnabled, ViewModel.ToggleRunningTimerNotifications))
                .Subscribe(runningTimeEntryRow.SetRowData)
                .DisposedBy(DisposeBag);
            
            ViewModel.AreStoppedTimerNotificationsEnabled
                .Select(stoppedTimerNotificationsEnabled => new ToggleRow(NotificationsRunningTimer, stoppedTimerNotificationsEnabled, ViewModel.ToggleStoppedTimerNotifications))
                .Subscribe(stoppedTimerRow.SetRowData)
                .DisposedBy(DisposeBag);

            // calendar section 
            
            calendarSettingsRow.SetRowData(new NavigationRow(CalendarSettingsTitle, ViewModel.OpenCalendarSettings));
            
            ViewModel.IsCalendarSmartRemindersVisible
                .Subscribe(smartRemindersRow.ItemView.Rx().IsVisible())
                .DisposedBy(DisposeBag);
            
            ViewModel.CalendarSmartReminders
                .Select(smartReminders => new NavigationRow(SmartReminders, smartReminders, ViewModel.OpenCalendarSmartReminders))
                .Subscribe(smartRemindersRow.SetRowData)
                .DisposedBy(DisposeBag);

            // general section
            submitFeedbackRow.SetRowData(new NavigationRow(SubmitFeedback, ViewModel.SubmitFeedback)); 
            aboutRow.SetRowData(new NavigationRow(About, ViewModel.Version, ViewModel.OpenAboutView)); 
            helpRow.SetRowData(new NavigationRow(Help, ViewModel.OpenHelpView));
            
            ViewModel.LoggingOut
                .Subscribe(this.CancelAllNotifications)
                .DisposedBy(DisposeBag);

            ViewModel.IsFeedbackSuccessViewShowing
                .Subscribe(showFeedbackSuccessToast)
                .DisposedBy(DisposeBag);

            ViewModel.CurrentSyncStatus
                .Select(syncStatus => new CustomRow<PresentableSyncStatus>(syncStatus, ViewModel.TryLogout))
                .Subscribe(logoutRowViewView.SetRowData)
                .DisposedBy(DisposeBag);
        }

        private void showFeedbackSuccessToast(bool succeeded)
        {
            if (!succeeded) return;

            var toast = Toast.MakeText(this, Shared.Resources.SendFeedbackSuccessMessage, ToastLength.Long);
            toast.SetGravity(GravityFlags.CenterHorizontal | GravityFlags.Bottom, 0, 0);
            toast.Show();
        }
    }
}
