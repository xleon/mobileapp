using System;
using System.Linq;
using Foundation;
using Intents;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Analytics;
using Toggl.iOS.Intents;
using Toggl.iOS.ViewControllers;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override bool ContinueUserActivity(
            UIApplication application, 
            NSUserActivity userActivity,
            UIApplicationRestorationHandler completionHandler)
        {
            var navigationService = IosDependencyContainer.Instance.NavigationService;

            var interaction = userActivity.GetInteraction();
            if (interaction == null || interaction.IntentHandlingStatus != INIntentHandlingStatus.DeferredToApplication)
            {
                return false;
            }

            var intent = interaction?.Intent;

            switch (intent)
            {
                // TODO: Reimplement when working on Deeplinks
                case StopTimerIntent _:
                    //navigationService.Navigate(ApplicationUrls.Main.StopFromSiri);
                    return true;
                case ShowReportIntent _:
                    //navigationService.Navigate(ApplicationUrls.Reports);
                    return true;
                case ShowReportPeriodIntent periodIntent:
                    var tabbarVC = (MainTabBarController)UIApplication.SharedApplication.KeyWindow.RootViewController;
                    //TODO: Figure out this when working on #4860
                    //var reportViewModel = (ReportsViewModel)tabbarVC.ViewModel.Tabs.Single(viewModel => viewModel is ReportsViewModel);
                    //navigationService.Navigate(reportViewModel, periodIntent.Period.ToReportPeriod());
                    return true;
                case StartTimerIntent startTimerIntent:
                    var timeEntryParams = createStartTimeEntryParameters(startTimerIntent);
                    navigationService.Navigate<MainViewModel>();
                    navigationService.Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(timeEntryParams);
                    return true;
                default:
                    return false;
            }
        }

        private StartTimeEntryParameters createStartTimeEntryParameters(StartTimerIntent intent)
        {
            var tags = (intent.Tags == null || intent.Tags.Count() == 0)
                ? null
                : intent.Tags.Select(tagid => (long)Convert.ToDouble(tagid.Identifier));

            return new StartTimeEntryParameters(
                DateTimeOffset.Now,
                "",
                null,
                string.IsNullOrEmpty(intent.Workspace?.Identifier) ? null : (long?)Convert.ToDouble(intent.Workspace?.Identifier),
                intent.EntryDescription ?? "",
                string.IsNullOrEmpty(intent.ProjectId?.Identifier) ? null : (long?)Convert.ToDouble(intent.ProjectId?.Identifier),
                tags,
                TimeEntryStartOrigin.Siri
            );
        }
    }
}
