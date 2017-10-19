// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Toggl.Daneel
{
	[Register ("SummaryPage")]
	partial class SummaryPage
	{
		[Outlet]
		UIKit.UILabel TimeLabel1 { get; set; }

		[Outlet]
		UIKit.UILabel TimeLabel2 { get; set; }

		[Outlet]
		UIKit.UILabel TimeLabel3 { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView Timeline { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Timeline != null) {
				Timeline.Dispose ();
				Timeline = null;
			}

			if (TimeLabel1 != null) {
				TimeLabel1.Dispose ();
				TimeLabel1 = null;
			}

			if (TimeLabel2 != null) {
				TimeLabel2.Dispose ();
				TimeLabel2 = null;
			}

			if (TimeLabel3 != null) {
				TimeLabel3.Dispose ();
				TimeLabel3 = null;
			}
		}
	}
}
