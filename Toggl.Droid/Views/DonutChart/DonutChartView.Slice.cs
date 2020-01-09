using Toggl.Shared;
using static Toggl.Core.UI.ViewModels.Reports.ReportDonutChartElement;

namespace Toggl.Droid.Views
{
    public sealed partial class DonutChartView
    {
        private class Slice
        {
            public PercentageDecoratedSegment PercentageSegment { get; private set; }

            public float OriginalPercentage { get; private set; }
            public float Percentage { get; private set; }
            public float StartAngle { get; private set; }
            public float SweepAngle { get; private set; }
            public float EndAngle { get; private set; }

            public Slice(PercentageDecoratedSegment percentageSegment, float startAngle, float sweepAngle)
            {
                PercentageSegment = percentageSegment;
                Percentage = (float)percentageSegment.NormalizedPercentage;
                OriginalPercentage = (float)PercentageSegment.OriginalPercentage;
                StartAngle = startAngle.NormalizedAngle();
                SweepAngle = sweepAngle.NormalizedAngle();
                EndAngle = (startAngle + sweepAngle).NormalizedAngle();
            }

            public bool ContainsAngle(float angle)
            {
                angle = angle.NormalizedAngle();
                return StartAngle <= angle && angle <= EndAngle;
            }

            /// <summary>
            /// Because of floating point arithmetic imprecision,
            /// it's possible that the sum of all angles is not exactly 360.
            /// This method snaps the end angle to 360.
            /// </summary>
            public void CorrectEndAngle()
            {
                EndAngle = 360;
                SweepAngle = EndAngle - StartAngle;
            }
        }
    }
}
