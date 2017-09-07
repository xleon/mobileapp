using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    [Register(nameof(TimeEntryTagsTextView))]
    public class TimeEntryTagsTextView : UITextView
    {
        public TimeEntryTagsTextView(IntPtr handle)
            : base(handle)
        {
        }
    }
}
