using ObjCRuntime;
using Foundation;
using System;
using UIKit;
using Toggl.Daneel.Extensions;
using System.Reactive;
using Toggl.Foundation;

namespace Toggl.Daneel
{
    public sealed partial class CalendarSettingsTableViewHeader : UIView
    {
        public IObservable<Unit> EnableCalendarAccessTapped { get; private set; }

        public CalendarSettingsTableViewHeader (IntPtr handle) : base (handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            EnableCalendarAccessTapped = EnableCalendarAccessView.Tapped();
        }

        public static CalendarSettingsTableViewHeader Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(CalendarSettingsTableViewHeader), null, null);
            return Runtime.GetNSObject<CalendarSettingsTableViewHeader>(arr.ValueAt(0));
        }

        public void SetCalendarPermissionStatus(bool enabled)
        {
            CalendarPermissionStatusLabel.Text = enabled
                ? Resources.On
                : Resources.Off;
        }
    }
}
