// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;

namespace Toggl.iOS.Cells.MainLog
{
    [Register("UserFeedbackTableViewCell")]
    partial class UserFeedbackTableViewCell
    {
        [Outlet]
        UIKit.UIView CardView { get; set; }

        [Outlet]
        UIKit.UIButton CallToActionButton { get; set; }

        [Outlet]
        UIKit.UILabel CallToActionDescription { get; set; }

        [Outlet]
        UIKit.UILabel CallToActionTitle { get; set; }

        [Outlet]
        UIKit.UIView CallToActionView { get; set; }

        [Outlet]
        UIKit.UIButton DismissButton { get; set; }

        [Outlet]
        UIKit.UILabel NotReallyLabel { get; set; }

        [Outlet]
        UIKit.UIView NotReallyView { get; set; }

        [Outlet]
        UIKit.UIView QuestionView { get; set; }

        [Outlet]
        UIKit.UILabel TitleLabel { get; set; }

        [Outlet]
        UIKit.UILabel YesLabel { get; set; }

        [Outlet]
        UIKit.UIView YesView { get; set; }

        void ReleaseDesignerOutlets()
        {
            if (CardView != null)
            {
                CardView.Dispose();
                CardView = null;
            }

            if (CallToActionButton != null)
            {
                CallToActionButton.Dispose();
                CallToActionButton = null;
            }

            if (CallToActionDescription != null)
            {
                CallToActionDescription.Dispose();
                CallToActionDescription = null;
            }

            if (CallToActionTitle != null)
            {
                CallToActionTitle.Dispose();
                CallToActionTitle = null;
            }

            if (CallToActionView != null)
            {
                CallToActionView.Dispose();
                CallToActionView = null;
            }

            if (DismissButton != null)
            {
                DismissButton.Dispose();
                DismissButton = null;
            }

            if (NotReallyLabel != null)
            {
                NotReallyLabel.Dispose();
                NotReallyLabel = null;
            }

            if (NotReallyView != null)
            {
                NotReallyView.Dispose();
                NotReallyView = null;
            }

            if (QuestionView != null)
            {
                QuestionView.Dispose();
                QuestionView = null;
            }

            if (TitleLabel != null)
            {
                TitleLabel.Dispose();
                TitleLabel = null;
            }

            if (YesLabel != null)
            {
                YesLabel.Dispose();
                YesLabel = null;
            }

            if (YesView != null)
            {
                YesView.Dispose();
                YesView = null;
            }
        }
    }
}

