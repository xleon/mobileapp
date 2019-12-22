// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.Views
{
	[Register ("DateRangePickerCell")]
	partial class DateRangePickerCell
	{
		[Outlet]
		Toggl.iOS.Views.RoundedView BackgroundView { get; set; }

		[Outlet]
		Toggl.iOS.Views.RoundedView LeftBackgroundView { get; set; }

		[Outlet]
		Toggl.iOS.Views.RoundedView RightBackgroundView { get; set; }

		[Outlet]
		UIKit.UILabel Text { get; set; }

		[Outlet]
		Toggl.iOS.Views.RoundedView TodayBackgroundView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (LeftBackgroundView != null) {
				LeftBackgroundView.Dispose ();
				LeftBackgroundView = null;
			}

			if (RightBackgroundView != null) {
				RightBackgroundView.Dispose ();
				RightBackgroundView = null;
			}

			if (BackgroundView != null) {
				BackgroundView.Dispose ();
				BackgroundView = null;
			}

			if (Text != null) {
				Text.Dispose ();
				Text = null;
			}

			if (TodayBackgroundView != null) {
				TodayBackgroundView.Dispose ();
				TodayBackgroundView = null;
			}
		}
	}
}
