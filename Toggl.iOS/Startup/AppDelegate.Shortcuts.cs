using System;
using Foundation;
using Toggl.Core;
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

            var urlHandler = IosDependencyContainer.Instance.UrlHandler;

            var shortcutUrlKey = new NSString(nameof(ApplicationShortcut.Url));
            if (!shortcutItem.UserInfo.ContainsKey(shortcutUrlKey))
                return;

            var shortcutUrlString = shortcutItem.UserInfo[shortcutUrlKey] as NSString;
            if (shortcutUrlString == null)
                return;

            var shortcutUrl = new NSUrl(shortcutUrlString);

            urlHandler.Handle(shortcutUrl);
        }
    }
}
