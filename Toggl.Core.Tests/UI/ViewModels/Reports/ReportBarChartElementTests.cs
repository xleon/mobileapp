using System;
using System.Collections.Generic;
using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;
using Xunit;
using Bar = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.Bar;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportBarChartElementTests
    {
        private static readonly double eps = 0.00001;
        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportBarChartElement.LoadingState.IsLoading.Should().BeTrue();
            }
        }

        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                new ReportBarChartElement(new List<Bar>(), DurationFormat.Classic).IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void NormalizesBars()
            {
                var element = setupReportBarChartElement(null);
             
                element.Bars[0].FilledValue.Should().BeApproximately(0.25, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);
                
                element.Bars[1].FilledValue.Should().BeApproximately(0.25, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
            }
            
            [Fact, LogIfTooSlow]
            public void UsesCustomNormalizeMapping()
            {
                var element = setupReportBarChartElement(bar => bar);
             
                element.Bars[0].FilledValue.Should().BeApproximately(1, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(2, eps);
                
                element.Bars[1].FilledValue.Should().BeApproximately(1, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(4, eps);
            }

            private ReportBarChartElement setupReportBarChartElement(Func<Bar, Bar> scalingFunction)
            {
                var offsetRange = new DateTimeOffsetRange(DateTimeOffset.Now, DateTimeOffset.Now);
                return new ReportBarChartElement(new Bar[]
                    {
                        new Bar(1, 2, offsetRange),
                        new Bar(1, 4, offsetRange)
                    },
                    DurationFormat.Classic,
                    scalingFunction);
            }
        }
    }
}