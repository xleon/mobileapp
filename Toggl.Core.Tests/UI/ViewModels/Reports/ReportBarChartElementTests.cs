using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;
using Bar = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.Bar;
using YAxisLabels = Toggl.Core.UI.ViewModels.Reports.ReportBarChartElement.YAxisLabels;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportBarChartElementTests
    {
        private const double eps = 0.00001;

        public class ReportBarChartElementBaseTests
        {
            protected ReportBarChartElement SetupReportBarChartElement(Func<Bar, Bar> scalingFunction = null)
            {
                var bars = new Bar[]
                {
                    new Bar(1, 2),
                    new Bar(1, 4)
                };

                var xLabels = new[] { "20-12", "28-12" };
                var yLabels = new YAxisLabels("10m", "5m", "0m");

                return new ReportBarChartElement(bars, xLabels, yLabels, scalingFunction);
            }

            protected ImmutableList<Bar> CreateBars(int count = 5)
                => Enumerable.Range(0, count).Select(i => new Bar(i, i * 2)).ToImmutableList();

            protected ImmutableList<string> CreateXLables(int count = 2)
                => Enumerable.Range(0, count).Select(i => i.ToString()).ToImmutableList();

            protected YAxisLabels CreateYAxisLabels(string unit = "h")
                => new YAxisLabels($"10 {unit}", $"5 {unit}", $"0 {unit}");
        }

        public sealed class TheLoadingStateProperty
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToTrue()
            {
                ReportBarChartElement.LoadingState.IsLoading.Should().BeTrue();
            }
        }

        public sealed class TheConstructor : ReportBarChartElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsIsLoadingToFalse()
            {
                var element = SetupReportBarChartElement();

                element.IsLoading.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void HasCorrectAmountOfBars()
            {
                var count = 11;
                var bars = CreateBars(count);
                var xLabels = CreateXLables();
                var yLabels = CreateYAxisLabels();
                var element = new ReportBarChartElement(bars, xLabels, yLabels);

                element.Bars.Should().HaveCount(count);
            }


            [Fact, LogIfTooSlow]
            public void NormalizesBars()
            {
                var element = SetupReportBarChartElement();

                element.Bars[0].FilledValue.Should().BeApproximately(0.25, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(0.5, eps);

                element.Bars[1].FilledValue.Should().BeApproximately(0.25, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(1, eps);
            }

            [Fact, LogIfTooSlow]
            public void UsesCustomNormalizeMapping()
            {
                var scaler = Substitute.For<Func<Bar,Bar>>();
                scaler(Arg.Any<Bar>()).Returns(c => (Bar)c.Args()[0]);

                var element = SetupReportBarChartElement(scaler);

                scaler.Received()(Arg.Any<Bar>());
                element.Bars[0].FilledValue.Should().BeApproximately(1, eps);
                element.Bars[0].TotalValue.Should().BeApproximately(2, eps);
                element.Bars[1].FilledValue.Should().BeApproximately(1, eps);
                element.Bars[1].TotalValue.Should().BeApproximately(4, eps);
            }
        }

        public sealed class TheEqualsMethod : ReportBarChartElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var bars = CreateBars();
                var xLabels = CreateXLables();
                var yLabels = CreateYAxisLabels();
                var elementA = new ReportBarChartElement(bars, xLabels, yLabels);
                var elementB = new ReportBarChartElement(bars, xLabels, yLabels);

                elementA.Equals(elementB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInIsLoading()
            {
                var bars = CreateBars();
                var xLabels = CreateXLables();
                var yLabels = CreateYAxisLabels();
                var elementA = new ReportBarChartElement(bars, xLabels, yLabels);
                var elementB = ReportBarChartElement.LoadingState;

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInBars()
            {
                var barsA = CreateBars(4);
                var barsB = CreateBars(2);
                var xLabels = CreateXLables();
                var yLabels = CreateYAxisLabels();
                var elementA = new ReportBarChartElement(barsA, xLabels, yLabels);
                var elementB = new ReportBarChartElement(barsB, xLabels, yLabels);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInXLabels()
            {
                var bars = CreateBars();
                var xLabelsA = CreateXLables(2);
                var xLabelsB = CreateXLables(4);
                var yLabels = CreateYAxisLabels();
                var elementA = new ReportBarChartElement(bars, xLabelsA, yLabels);
                var elementB = new ReportBarChartElement(bars, xLabelsB, yLabels);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInYLabels()
            {
                var bars = CreateBars();
                var xLabels = CreateXLables();
                var yLabelsA = CreateYAxisLabels("h");
                var yLabelsB = CreateYAxisLabels("min");
                var elementA = new ReportBarChartElement(bars, xLabels, yLabelsA);
                var elementB = new ReportBarChartElement(bars, xLabels, yLabelsB);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var bars = CreateBars();
                var xLabels = CreateXLables();
                var yLabels = CreateYAxisLabels();
                var element = new ReportBarChartElement(bars, xLabels, yLabels);
                var noDataElement = new ReportNoDataElement();

                element.Equals(noDataElement).Should().BeFalse();
            }
        }
    }
}
