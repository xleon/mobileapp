using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using MvvmCross.Plugins.Color.iOS;
using ObjCRuntime;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel
{
    public sealed partial class SummaryPage : UIView
    {
        private const int timelineSeparatorCount = 11;
        private double timelineSeparatorSpacing;

        public SummaryPage (IntPtr handle) : base (handle)
        {
        }

        public static SummaryPage Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(SummaryPage), null, null);
            return Runtime.GetNSObject<SummaryPage>(arr.ValueAt(0));
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            drawTimelineSeparators();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            timelineSeparatorSpacing
                 = Math.Ceiling((Timeline.Bounds.Width - timelineSeparatorCount) / (timelineSeparatorCount + 1)) + 1;

            positionTimeLabels();
        }

        private void drawTimelineSeparators()
        {
            var layer = new CAShapeLayer
            {
                Path = createSeparatorPath(),
                StrokeColor = Color.Onboarding.SummaryPageTimelineSeparators.ToNativeColor().CGColor,
                LineWidth = 1,
                Position = Timeline.Bounds.Location
            };
            Layer.AddSublayer(layer);
        }

        private CGPath createSeparatorPath()
        {
            var currentXPosition = timelineSeparatorSpacing;
            var topYPosition = Timeline.Frame.Top + 1;
            var bottomYPosition = Timeline.Frame.Bottom;
            var path = new UIBezierPath();

            //Horizontal line
            path.MoveTo(new CGPoint(Timeline.Frame.Left, Timeline.Frame.Top + 0.5f));
            path.AddLineTo(new CGPoint(Timeline.Frame.Right, Timeline.Frame.Top + 0.5f));

            //Vertical lines
            for (int i = 0; i < timelineSeparatorCount; i++)
            {
                path.MoveTo(new CGPoint(currentXPosition, topYPosition));
                path.AddLineTo(new CGPoint(currentXPosition, bottomYPosition));
                currentXPosition += timelineSeparatorSpacing;
            }

            return path.CGPath;
        }

        private void positionTimeLabels()
        {
            //Labels are positioned below 2nd, 6th and 10th vertical separator line
            var position = 2 * timelineSeparatorSpacing + 1;
            TimeLabel1.CenterXAnchor
                .ConstraintEqualTo(LeadingAnchor, (nfloat)position).Active = true;

            position = 6 * timelineSeparatorSpacing + 1;
            TimeLabel2.CenterXAnchor
                .ConstraintEqualTo(LeadingAnchor, (nfloat)position).Active = true;

            position = 10 * timelineSeparatorSpacing + 1;
            TimeLabel3.CenterXAnchor
                .ConstraintEqualTo(LeadingAnchor, (nfloat)position).Active = true;
        }
    }
}
