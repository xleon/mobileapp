using Android.Widget;
using AndroidX.Core.Widget;
using Google.Android.Material.AppBar;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHolders.Settings;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

namespace Toggl.Droid.Activities
{
    public partial class SettingsActivity
    {
        private AppBarLayout appBarLayout;
        private NestedScrollView scrollView;
        private Toolbar toolbar;

        private LinearLayout settingsContainer;
        private InfoRowViewView nameRow;
        private InfoRowViewView emailRow;
        private NavigationRowViewView workspaceRow;
        private NavigationRowViewView dateFormatRow;
        private ToggleRowViewView use24HoursFormatRow;
        private NavigationRowViewView durationFormatRow;
        private NavigationRowViewView beginningOfWeekRow;
        private ToggleRowViewView isGroupingTimeEntriesRow;
        private ToggleRowViewView swipeActionsRow;
        private ToggleRowViewView runningTimeEntryRow;
        private ToggleRowViewView stoppedTimerRow;
        private ToggleRowViewWithDescriptionView isManualModeEnabledRowView;
        private NavigationRowViewView calendarSettingsRow;
        private NavigationRowViewView smartRemindersRow;
        private NavigationRowViewView submitFeedbackRow;
        private NavigationRowViewView aboutRow;
        private NavigationRowViewView helpRow;
        private LogoutRowViewView logoutRowViewView;

        protected override void InitializeViews()
        {
            appBarLayout = FindViewById<AppBarLayout>(Resource.Id.AppBarLayout);
            scrollView = FindViewById<NestedScrollView>(Resource.Id.ScrollView);
            settingsContainer = FindViewById<LinearLayout>(Resource.Id.settingsContainer);

            nameRow = InfoRowViewView.Create(this);
            emailRow = InfoRowViewView.Create(this);
            workspaceRow = NavigationRowViewView.Create(this);
            dateFormatRow = NavigationRowViewView.Create(this);
            use24HoursFormatRow = ToggleRowViewView.Create(this);
            durationFormatRow = NavigationRowViewView.Create(this);
            beginningOfWeekRow = NavigationRowViewView.Create(this);
            isGroupingTimeEntriesRow = ToggleRowViewView.Create(this);
            swipeActionsRow = ToggleRowViewView.Create(this);
            isManualModeEnabledRowView = ToggleRowViewWithDescriptionView.Create(this);
            runningTimeEntryRow = ToggleRowViewView.Create(this);
            stoppedTimerRow = ToggleRowViewView.Create(this);
            calendarSettingsRow = NavigationRowViewView.Create(this, viewLayout: Resource.Layout.SettingsButtonRowView);
            smartRemindersRow = NavigationRowViewView.Create(this);
            submitFeedbackRow = NavigationRowViewView.Create(this, viewLayout: Resource.Layout.SettingsButtonRowView);
            aboutRow = NavigationRowViewView.Create(this, viewLayout: Resource.Layout.SettingsButtonRowWithDetailsView);
            helpRow = NavigationRowViewView.Create(this, viewLayout: Resource.Layout.SettingsButtonRowView);
            logoutRowViewView = LogoutRowViewView.Create(this);

            settingsContainer.AddView(nameRow.ItemView);
            settingsContainer.AddView(emailRow.ItemView);
            settingsContainer.AddView(workspaceRow.ItemView);
            settingsContainer.AddView(dateFormatRow.ItemView);
            settingsContainer.AddView(use24HoursFormatRow.ItemView);
            settingsContainer.AddView(durationFormatRow.ItemView);
            settingsContainer.AddView(beginningOfWeekRow.ItemView);
            settingsContainer.AddView(isGroupingTimeEntriesRow.ItemView);
            settingsContainer.AddView(swipeActionsRow.ItemView);
            settingsContainer.AddView(isManualModeEnabledRowView.ItemView);
            settingsContainer.AddView(calendarSettingsRow.ItemView);
            settingsContainer.AddView(smartRemindersRow.ItemView);
            settingsContainer.AddView(submitFeedbackRow.ItemView);
            settingsContainer.AddView(aboutRow.ItemView);
            settingsContainer.AddView(helpRow.ItemView);
            settingsContainer.AddView(logoutRowViewView.ItemView);

            toolbar = FindViewById<Toolbar>(Resource.Id.Toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = Shared.Resources.Settings;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);

            var baseMargin = 24.DpToPixels(this);
            logoutRowViewView.ItemView.DoOnApplyWindowInsets((v, insets, initialPadding) =>
            {
                var bottomMargin = baseMargin + insets.SystemWindowInsetBottom;
                var currentLayoutParams = logoutRowViewView.ItemView.LayoutParameters as LinearLayout.LayoutParams;
                logoutRowViewView.ItemView.LayoutParameters = currentLayoutParams.WithMargins(bottom: bottomMargin);
            });
        }
    }
}