using CoreGraphics;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.iOS.Extensions;
using UIKit;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;
using static Toggl.Shared.Math;
using Color = Toggl.Shared.Color;
using Math = System.Math;

namespace Toggl.iOS.Views.Reports
{
    [Register(nameof(DonutChartView))]
    public sealed class DonutChartView : UIView
    {
        private const float donutInnerCircleFactor = 0.6f;

        private float centerX;
        private float centerY;
        private float radius;
        private float innerRadius;
        private float textRadius;
        private float totalValue;

        private static readonly UIStringAttributes attributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(10, UIFontWeight.Semibold),
            ForegroundColor = UIColor.White
        };

        private IEnumerable<Segment> segments = new Segment[0];
        public IEnumerable<Segment> Segments
        {
            get => segments;
            set
            {
                segments = value;
                SetNeedsDisplay();
            }
        }

        public DonutChartView(IntPtr handle) : base(handle)
        {
        }

        public override void Draw(CGRect rect)
        {
            var ctx = UIGraphics.GetCurrentContext();
            if (ctx == null) return;

            centerX = (float) Bounds.Size.Width * 0.5f;
            centerY = (float) Bounds.Size.Height * 0.5f;
            radius = centerX;
            innerRadius = centerX * donutInnerCircleFactor;
            textRadius = (radius + innerRadius) / 2;
            totalValue = (float) Segments.Sum(s => s.Value);

            var startAngle = (float) Math.PI * -0.5f;

            foreach (var segment in Segments)
            {
                drawSegment(ctx, segment, ref startAngle);
            }

            drawInnerCircle(ctx);
        }

        private void drawSegment(CGContext ctx, Segment segment, ref float startAngle)
        {
            ctx.SetFillColor(new Color(segment.Color).ToNativeColor().CGColor);

            var percent = (float)segment.Value / totalValue;
            var endAngle = startAngle + (float)FullCircle * percent;

            ctx.MoveTo(centerX, centerY);
            ctx.AddArc(centerX, centerY, radius, startAngle, endAngle, clockwise: false);
            ctx.FillPath();

            drawLabel(ctx, percent, startAngle, endAngle);

            startAngle = endAngle;
        }

        private void drawLabel(CGContext ctx, float percent, float startAngle, float endAngle)
        {
            var textAngle = (startAngle + endAngle) / 2;
            var integerPercentage = (int)(percent * 100);
            var percentageToDraw = new NSAttributedString($"{integerPercentage}%", attributes);
            ctx.MoveTo(centerX, centerY);
            ctx.AddArc(centerX, centerY, textRadius, textAngle, textAngle, clockwise: false);
            var textPoint = ctx.GetPathCurrentPoint();
            var percentWidth = percentageToDraw.Size.Width;
            var percentHeight = percentageToDraw.Size.Height;
            percentageToDraw.DrawString(new CGPoint(textPoint.X - percentWidth / 2, textPoint.Y - percentHeight / 2));
        }

        private void drawInnerCircle(CGContext ctx)
        {
            ctx.MoveTo(centerX, centerY);
            ctx.AddArc(centerX, centerY, innerRadius, 0, (float)FullCircle, clockwise: false);
            ctx.SetFillColor(BackgroundColor.CGColor);
            ctx.FillPath();
        }
    }
}
