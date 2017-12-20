using System;

namespace Toggl.Foundation.Reports
{
    public struct ChartSegment
    {
        public TimeSpan TrackedTime { get; }

        public float BillableSeconds { get; }

        public float Percentage { get; }

        public string ProjectName { get; }

        public string Color { get; }
        
        public ChartSegment(string projectName, float percentage, float trackedSeconds, float billableSeconds, string color)
        {
            ProjectName = projectName;
            Color = color;
            Percentage = percentage;
            TrackedTime = TimeSpan.FromSeconds(trackedSeconds);
            BillableSeconds = billableSeconds;
        }
    }
}
