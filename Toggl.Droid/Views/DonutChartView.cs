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

        private Paint paint = new Paint() { AntiAlias = true };
        private Color backgroundColor;

        private ReportDonutChartDonutElement data;

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
        }

        public void Update(ReportDonutChartDonutElement data)
        {
            this.data = data;
            PostInvalidate();
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, widthMeasureSpec);
        }

        protected override void OnDraw(Canvas canvas)
        {
            const float angleRightToTopCorrection = -90;

            if (data == null)
                return;

            var totalValue = data.Segments.Sum(s => s.Value);

            var angleOffset = angleRightToTopCorrection;

            using (var circle = new RectF(0, 0, Width, Height))
            {
                foreach (var segment in data.Segments)
                {
                    drawSlice(canvas, circle, segment, totalValue, ref angleOffset);
                }
            }

            drawInnerCircle(canvas);
        }

        private void drawSlice(Canvas canvas, RectF circle, Segment segment, double totalValue, ref float angleOffset)
        {
            var sliceAngle = (float)(fullCircle * segment.Value / totalValue);
            paint.Color = Color.ParseColor(segment.Color);

            canvas.DrawArc(circle, angleOffset, sliceAngle, true, paint);

            angleOffset += sliceAngle;
        }

        private void drawInnerCircle(Canvas canvas)
        {
            paint.Color = backgroundColor;

            canvas.DrawCircle(Width / 2, Width / 2, Width * donutInnerCircleFactor / 2, paint);
        }
    }
}
