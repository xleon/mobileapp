using System;
using Foundation;
using Toggl.Core.Shortcuts;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using UIKit;

namespace Toggl.iOS
{
    public partial class AppDelegate
    {
        public override void PerformActionForShortcutItem(UIApplication application, UIApplicationShortcutItem shortcutItem, UIOperationHandler completionHandler)
        {
            IosDependencyContainer.Instance
                .AnalyticsService
                .ApplicationShortcut
                .Track(shortcutItem.LocalizedTitle);

            var navigationService = IosDependencyContainer.Instance.NavigationService;

            var key = new NSString(nameof(ApplicationShortcut.Type));
            if (!shortcutItem.UserInfo.ContainsKey(key))
                return;

            var shortcutNumber = shortcutItem.UserInfo[key] as NSNumber;
            if (shortcutNumber == null)
                return;

            var shortcutType = (ShortcutType)(int)shortcutNumber;

            switch (shortcutType)
            {
                case ShortcutType.ContinueLastTimeEntry:
                    navigationService.Navigate<MainViewModel>();
                    var interactorFactory = IosDependencyContainer.Instance.InteractorFactory;
                    if (interactorFactory == null) return;
                    IDisposable subscription = null;
                    subscription = interactorFactory
                        .ContinueMostRecentTimeEntry()
                        .Execute()
                        .Subscribe(_ =>
                        {
                            subscription.Dispose();
                            subscription = null;
                        });
                    break;

                case ShortcutType.Reports:
                    navigationService.Navigate<ReportsViewModel>();
                    break;

                case ShortcutType.StartTimeEntry:
                    navigationService.Navigate<MainViewModel>();
                    navigationService.Navigate<StartTimeEntryViewModel, StartTimeEntryParameters>(StartTimeEntryParameters.ForTimerMode(DateTime.UtcNow));
                    break;

                case ShortcutType.Calendar:
                    navigationService.Navigate<CalendarViewModel>();
                    break;
            }
        }
    }
}
