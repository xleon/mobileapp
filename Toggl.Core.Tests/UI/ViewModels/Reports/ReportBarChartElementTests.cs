using System;
using System.Collections.Generic;
using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;
using Bar = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.Bar;
using YAxisLabels = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.YAxisLabels;

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
                var element = new ReportBarChartElement(
                    new List<Bar>(),
                    new string[] { "aaa", "aa", "" },
                    new YAxisLabels("aaa", "aa", "" ));

                element.IsLoading.Should().BeFalse();
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
                return new ReportBarChartElement(new Bar[]
                    {
                        new Bar(1, 2),
                        new Bar(1, 4)
                    },
                    new string[] { "aaa", "aa", "" },
                    new YAxisLabels("aaa", "aa", ""),
                    scalingFunction);
            }
        }
    }
}