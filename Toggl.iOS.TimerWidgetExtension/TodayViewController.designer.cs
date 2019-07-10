// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.iOS.TimerWidgetExtension
{
	[Register ("TodayViewController")]
	partial class TodayViewController
	{
		[Outlet]
		UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
		UIKit.UIView DotView { get; set; }

		[Outlet]
		UIKit.UILabel DurationLabel { get; set; }

		[Outlet]
		UIKit.UILabel ProjectNameLabel { get; set; }

		[Outlet]
		UIKit.UIView RunningTimerContainerView { get; set; }

		[Outlet]
		UIKit.UIButton StartButton { get; set; }

		[Outlet]
		UIKit.UIButton StopButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DurationLabel != null) {
				DurationLabel.Dispose ();
				DurationLabel = null;
			}

			if (RunningTimerContainerView != null) {
				RunningTimerContainerView.Dispose ();
				RunningTimerContainerView = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (ProjectNameLabel != null) {
				ProjectNameLabel.Dispose ();
				ProjectNameLabel = null;
			}

			if (DotView != null) {
				DotView.Dispose ();
				DotView = null;
			}

			if (StartButton != null) {
				StartButton.Dispose ();
				StartButton = null;
			}

			if (StopButton != null) {
				StopButton.Dispose ();
				StopButton = null;
			}
		}
	}
}
