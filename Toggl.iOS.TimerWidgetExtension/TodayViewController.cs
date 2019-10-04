using System;
using Foundation;
using NotificationCenter;
using Toggl.iOS.ExtensionKit;
using UIKit;

namespace Toggl.iOS.TimerWidgetExtension
{
    public partial class TodayViewController : UIViewController, INCWidgetProviding
    {

        private readonly NetworkingHandler networkingHandler
            = new NetworkingHandler(APIHelper.GetTogglAPI());

        protected TodayViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            StopButton.AddTarget(stopTimeEntry, UIControlEvent.TouchUpInside);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            SharedStorage.Instance.SetWidgetUpdatedDate(DateTimeOffset.Now);
        }

        private async void stopTimeEntry(object sender, EventArgs e)
        {
            await networkingHandler.StopRunningTimeEntry();
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            completionHandler(NCUpdateResult.NewData);
        }
    }
}
