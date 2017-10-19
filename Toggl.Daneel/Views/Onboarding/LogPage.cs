using Foundation;
using System;
using UIKit;
using ObjCRuntime;

namespace Toggl.Daneel
{
    public sealed partial class LogPage : UIView
    {
        public LogPage (IntPtr handle) : base (handle)
        {
        }

        public static LogPage Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(LogPage), null, null);
            return Runtime.GetNSObject<LogPage>(arr.ValueAt(0));
        }
    }
}
