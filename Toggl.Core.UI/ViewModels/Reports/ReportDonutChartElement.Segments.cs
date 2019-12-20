using System;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public partial class ReportDonutChartElement
    {
        public class Segment
        {
            public string Color { get; }
            public string Label { get; }
            public double Value { get; }

            public Segment(string color, string label, double value)
            {
                Color = color;
                Label = label;
                Value = value;
            }

            public override bool Equals(object obj)
                => GetType() == obj.GetType()
                && obj is Segment segment
                && segment.Color == Color
                && segment.Label == Label
                && segment.Value == Value;

            public override int GetHashCode()
                => HashCode.Combine(Color, Label, Value);

            public static bool operator ==(Segment left, Segment right)
                => left.Equals(right);

            public static bool operator !=(Segment left, Segment right)
                => !(left == right);
        }

        public class PercentageDecoratedSegment
        {
            public Segment Segment { get; }
            public double OriginalPercentage { get; }
            public double NormalizedPercentage { get; private set; }

            public PercentageDecoratedSegment(Segment segment, double originalPercentage)
            {
                Segment = segment;
                OriginalPercentage = originalPercentage;
                NormalizedPercentage = originalPercentage;
            }

            public PercentageDecoratedSegment WithNormalizedPercentage(double normalizedPercentage)
                => new PercentageDecoratedSegment(Segment, OriginalPercentage)
                {
                    NormalizedPercentage = normalizedPercentage
                };
        }
    }
}
