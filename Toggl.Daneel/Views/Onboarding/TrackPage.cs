using System;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace Toggl.Daneel
{
    public sealed partial class TrackPage : UIView
    {
        public TrackPage (IntPtr handle) : base (handle)
        {
        }

        public static TrackPage Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(TrackPage), null, null);
            return Runtime.GetNSObject<TrackPage>(arr.ValueAt(0));
        }
    }
}
