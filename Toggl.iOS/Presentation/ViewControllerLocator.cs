using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.ViewControllers;
using Toggl.iOS.ViewControllers.Calendar;
using Toggl.iOS.ViewControllers.Settings;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public static class ViewControllerLocator
    {
        public static UIViewController GetViewController<TViewModel>(TViewModel viewModel, bool shouldWrapInNavigationController = false) where TViewModel : IViewModel
        {
            UIViewController viewController = null;

            switch (viewModel)
            {
                case AboutViewModel aboutViewModel:
                    viewController = new AboutViewController(aboutViewModel);
                    break;
                case BrowserViewModel browserViewModel:
                    viewController = new BrowserViewController(browserViewModel);
                    break;
                case CalendarViewModel calendarViewModel:
                    viewController = new CalendarViewController(calendarViewModel);
                    break;
                case CalendarPermissionDeniedViewModel calendarPermissionDeniedViewModel:
                    viewController = new CalendarPermissionDeniedViewController(calendarPermissionDeniedViewModel);
                    break;
                case CalendarSettingsViewModel calendarSettingsViewModel:
                    viewController = new CalendarSettingsViewController(calendarSettingsViewModel);
                    break;
                case EditDurationViewModel editDurationViewModel:
                    viewController = new EditDurationViewController(editDurationViewModel);
                    break;
                case EditProjectViewModel editProjectViewModel:
                    viewController = new EditProjectViewController(editProjectViewModel);
                    break;
                case EditTimeEntryViewModel editTimeEntryViewModel:
                    viewController = new EditTimeEntryViewController(editTimeEntryViewModel);
                    break;
                case ForgotPasswordViewModel forgotPasswordViewModel:
                    viewController = new ForgotPasswordViewController(forgotPasswordViewModel);
                    break;
                case LicensesViewModel licensesViewModel:
                    viewController = new LicensesViewController(licensesViewModel);
                    break;
                case LoginViewModel loginViewModel:
                    viewController = LoginViewController.NewInstance(loginViewModel);
                    break;
                case MainTabBarViewModel mainTabBarViewModel:
                    viewController = new MainTabBarController(mainTabBarViewModel);
                    break;
                case MainViewModel mainViewModel:
                    viewController = new MainViewController(mainViewModel);
                    break;
                case NotificationSettingsViewModel notificationSettingsViewModel:
                    viewController = new NotificationSettingsViewController(notificationSettingsViewModel);
                    break;
                case NoWorkspaceViewModel noWorkspaceViewModel:
                    viewController = new NoWorkspaceViewController(noWorkspaceViewModel);
                    break;
                case OnboardingViewModel onboardingViewModel:
                    viewController = new OnboardingViewController(onboardingViewModel);
                    break;
                case OutdatedAppViewModel outdatedAppViewModel:
                    viewController = new OutdatedAppViewController(outdatedAppViewModel);
                    break;
                case ReportsViewModel reportsViewModel:
                    viewController = new ReportsViewController(reportsViewModel);
                    break;
                case ReportsCalendarViewModel reportsCalendarViewModel:
                    viewController = new ReportsCalendarViewController(reportsCalendarViewModel);
                    break;
                case SelectBeginningOfWeekViewModel selectBeginningOfWeekViewModel:
                    viewController = new SelectBeginningOfWeekViewController(selectBeginningOfWeekViewModel);
                    break;
                case SelectClientViewModel selectClientViewModel:
                    viewController = new SelectClientViewController(selectClientViewModel);
                    break;
                case SelectColorViewModel selectColorViewModel:
                    viewController = new SelectColorViewController(selectColorViewModel);
                    break;
                case SelectCountryViewModel selectCountryViewModel:
                    viewController = new SelectCountryViewController(selectCountryViewModel);
                    break;
                case SelectDateFormatViewModel selectDateFormatViewModel:
                    viewController = new SelectDateFormatViewController(selectDateFormatViewModel);
                    break;
                case SelectDateTimeViewModel selectDateTimeViewModel:
                    viewController = new SelectDateTimeViewController(selectDateTimeViewModel);
                    break;
                case SelectDefaultWorkspaceViewModel selectDefaultWorkspaceViewModel:
                    viewController = new SelectDefaultWorkspaceViewController(selectDefaultWorkspaceViewModel);
                    break;
                case SelectDurationFormatViewModel selectDurationFormatViewModel:
                    viewController = new SelectDurationFormatViewController(selectDurationFormatViewModel);
                    break;
                case SelectProjectViewModel selectProjectViewModel:
                    viewController = new SelectProjectViewController(selectProjectViewModel);
                    break;
                case SelectTagsViewModel selectTagsViewModel:
                    viewController = new SelectTagsViewController(selectTagsViewModel);
                    break;
                case SelectUserCalendarsViewModel selectUserCalendarsViewModel:
                    viewController = new SelectUserCalendarsViewController(selectUserCalendarsViewModel);
                    break;
                case SelectWorkspaceViewModel selectWorkspaceViewModel:
                    viewController = new SelectWorkspaceViewController(selectWorkspaceViewModel);
                    break;
                case SendFeedbackViewModel sendFeedbackViewModel:
                    viewController = new SendFeedbackViewController(sendFeedbackViewModel);
                    break;
                case SettingsViewModel settingsViewModel:
                    viewController = new SettingsViewController(settingsViewModel);
                    break;
                case SignupViewModel signupViewModel:
                    viewController = new SignupViewController(signupViewModel);
                    break;
                case StartTimeEntryViewModel startTimeEntryViewModel:
                    viewController = new StartTimeEntryViewController(startTimeEntryViewModel);
                    break;
                case SyncFailuresViewModel syncFailuresViewModel:
                    viewController = new SyncFailuresViewController(syncFailuresViewModel);
                    break;
                case TermsOfServiceViewModel termsOfServiceViewModel:
                    viewController = new TermsOfServiceViewController(termsOfServiceViewModel);
                    break;
                case TokenResetViewModel tokenResetViewModel:
                    viewController = new TokenResetViewController(tokenResetViewModel);
                    break;
                case UpcomingEventsNotificationSettingsViewModel upcomingEventsNotificationSettingsViewModel:
                    viewController = new UpcomingEventsNotificationSettingsViewController(upcomingEventsNotificationSettingsViewModel);
                    break;
                default:
                    throw new Exception($"Failed to create ViewController for ViewModel of type {viewModel.GetType().Name}");
            }

            return shouldWrapInNavigationController
                ? wrapInNavigationController(viewController)
                : viewController;
        }

        private static UIViewController wrapInNavigationController(UIViewController viewController)
            => new UINavigationController(viewController);
    }
}
