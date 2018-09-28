// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Cells.Calendar
{
    [Register ("SelectableUserCalendarViewCell")]
    partial class SelectableUserCalendarViewCell
    {
        [Outlet]
        UIKit.UILabel CalendarNameLabel { get; set; }


        [Outlet]
        UIKit.UISwitch IsSelectedSwitch { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CalendarNameLabel != null) {
                CalendarNameLabel.Dispose ();
                CalendarNameLabel = null;
            }

            if (IsSelectedSwitch != null) {
                IsSelectedSwitch.Dispose ();
                IsSelectedSwitch = null;
            }
        }
    }
}