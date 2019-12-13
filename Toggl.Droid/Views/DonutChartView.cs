using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Reports;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;
using Point = Toggl.Shared.Point;
using Toggl.Core.UI.Helper;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.DonutChartView")]
    public sealed class DonutChartView : View
    {
        private const float fullCircle = 360f;
        private const float donutInnerCircleFactor = 0.6f;
        private const float minimumSegmentPercentageToShowLabel = 0.06f;
        const float angleRightToTopCorrection = -90;
        const float degreesToRadians = MathF.PI / 180;
        private const float sliceLabelCenterRadius = (1 + donutInnerCircleFactor) / 2;

        private Paint paint = new Paint() { AntiAlias = true };
        private Paint textPaint = new Paint() { AntiAlias = true, TextAlign = Paint.Align.Center };
        private Rect textBounds = new Rect();

        private Color backgroundColor;
        private Color donutColor;

        private RectF circle = new RectF();

        private ReportDonutChartDonutElement data;
        private ImmutableList<PercentageDecoratedSegment> percentageSegments;

        public DonutChartView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public DonutChartView(Context context)
            : base(context)
        {
            initialize(context);
        }

        public DonutChartView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            initialize(context);
        }

        private void initialize(Context context)
        {
            backgroundColor = context.SafeGetColor(Resource.Color.cardBackground);
            donutColor = context.SafeGetColor(Resource.Color.placeholderDonut);

            textPaint.TextSize = 12.SpToPixels(context);
        }

        public void Update(ReportDonutChartDonutElement data)
        {
            this.data = data;
            percentageSegments = DonutChartGrouping.Group(data.Segments);

            PostInvalidate();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        protected override void OnDraw(Canvas canvas)
        {
            if (data == null)
                return;

            if (data.IsLoading)
            {
                drawIsLoading(canvas);
                return;
            }

            var angleOffset = angleRightToTopCorrection;

            circle.Set(0, 0, Width, Height);

            foreach (var segment in percentageSegments)
            {
                drawSlice(canvas, circle, segment, ref angleOffset);
            }

            drawInnerCircle(canvas);
        }

        private void drawIsLoading(Canvas canvas)
        {
            paint.Color = donutColor;

            canvas.DrawCircle(Width / 2, Height / 2, Width / 2, paint);

            drawInnerCircle(canvas);
        }

        private void drawSlice(Canvas canvas, RectF circle, PercentageDecoratedSegment percentageSegment, ref float angleOffset)
        {
            var sliceAngle = (float)(fullCircle * percentageSegment.NormalizedPercentage);
            paint.Color = Color.ParseColor(percentageSegment.Segment.Color);

            canvas.DrawArc(circle, angleOffset, sliceAngle, true, paint);

            drawSegmentPercentageLabel(canvas, percentageSegment.OriginalPercentage, sliceAngle, angleOffset);

            angleOffset += sliceAngle;
        }

        private void drawSegmentPercentageLabel(Canvas canvas, double percentage, float sliceAngle, float angleOffset)
        {
            if (percentage > minimumSegmentPercentageToShowLabel)
            {
                var labelAngle = angleOffset + sliceAngle / 2;
                var x = Width / 2 * (1 + sliceLabelCenterRadius * MathF.Cos(labelAngle * degreesToRadians));
                var y = Height / 2 * (1 + sliceLabelCenterRadius * MathF.Sin(labelAngle * degreesToRadians));

                drawLabel(canvas, $"{(int)(100 * percentage)}%", new Point(x, y));
            }
        }

        private void drawInnerCircle(Canvas canvas)
        {
            paint.Color = backgroundColor;

            canvas.DrawCircle(Width / 2, Width / 2, Width * donutInnerCircleFactor / 2, paint);
        }

        private void drawLabel(Canvas canvas, string text, Point center)
        {
            textPaint.GetTextBounds(text, 0, text.Length, textBounds);

            var offsetY = textBounds.Height() / 2;

            textPaint.Color = Color.White;
            canvas.DrawText(text, (float)center.X, (float)(center.Y + offsetY), textPaint);
        }
    }
}
