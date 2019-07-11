using System;
using NotificationCenter;
using Foundation;
using Toggl.iOS.ExtensionKit;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public partial class TodayViewController : UIViewController, INCWidgetProviding
    {
        protected TodayViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SharedStorage.Instance.SetWidgetUpdatedDate(DateTimeOffset.Now);
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            completionHandler(NCUpdateResult.NewData);
        }
    }
}
