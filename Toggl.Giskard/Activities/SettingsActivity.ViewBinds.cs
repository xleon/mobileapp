using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Toggl.Giskard.Activities
{
    public partial class SettingsActivity
    {
        private View manualModeView;
        private View avatarContainer;
        private View beginningOfWeekView;
        private View aboutContainer;

        private TextView nameTextView;
        private TextView emailTextView;
        private TextView beginningOfWeekTextView;
        private TextView versionTextView;

        private ImageView avatarView;

        private Button helpButton;
        private Button logoutButton;
        private Button feedbackButton;

        private Switch manualModeSwitch;

        private RecyclerView workspacesRecyclerView;

        protected override void InitializeViews()
        {
            manualModeView = FindViewById(Resource.Id.SettingsToggleManualModeView);
            avatarContainer = FindViewById(Resource.Id.SettingsViewAvatarImageContainer);
            beginningOfWeekView = FindViewById(Resource.Id.SettingsSelectBeginningOfWeekView);
            aboutContainer = FindViewById(Resource.Id.SettingsAboutContainer);

            nameTextView = FindViewById<TextView>(Resource.Id.SettingsNameTextView);
            emailTextView = FindViewById<TextView>(Resource.Id.SettingsEmailTextView);
            beginningOfWeekTextView = FindViewById<TextView>(Resource.Id.SettingsBeginningOfWeekTextView);
            versionTextView = FindViewById<TextView>(Resource.Id.SettingsAppVersionTextView);

            helpButton = FindViewById<Button>(Resource.Id.SettingsHelpButton);
            logoutButton = FindViewById<Button>(Resource.Id.SettingsLogoutButton);
            feedbackButton = FindViewById<Button>(Resource.Id.SettingsSubmitFeedbackButton);

            avatarView = FindViewById<ImageView>(Resource.Id.SettingsViewAvatarImage);
            manualModeSwitch = FindViewById<Switch>(Resource.Id.SettingsIsManualModeEnabledSwitch);

            workspacesRecyclerView = FindViewById<RecyclerView>(Resource.Id.SettingsWorkspacesRecyclerView);
        }
    }
}
