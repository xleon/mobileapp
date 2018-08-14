// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel.Cells.Calendar
{
	[Register ("CalendarItemView")]
	partial class CalendarItemView
	{
		[Outlet]
		UIKit.UIImageView CalendarIconImageView { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconLeadingConstraint { get; set; }

		[Outlet]
		UIKit.NSLayoutConstraint CalendarIconTrailingConstraint { get; set; }

		[Outlet]
		UIKit.UIView ColorView { get; set; }

		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalendarIconImageView != null) {
				CalendarIconImageView.Dispose ();
				CalendarIconImageView = null;
			}

			if (CalendarIconLeadingConstraint != null) {
				CalendarIconLeadingConstraint.Dispose ();
				CalendarIconLeadingConstraint = null;
			}

			if (CalendarIconTrailingConstraint != null) {
				CalendarIconTrailingConstraint.Dispose ();
				CalendarIconTrailingConstraint = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (ColorView != null) {
				ColorView.Dispose ();
				ColorView = null;
			}
		}
	}
}
