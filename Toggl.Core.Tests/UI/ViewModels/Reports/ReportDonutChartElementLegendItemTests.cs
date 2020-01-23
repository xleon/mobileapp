using System;
using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;
using FsCheck.Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportDonutChartElementLegendItemTests
    {
        public class ReportDonutChartLegendItemElementsBaseTests
        {
            public string Name { get; } = "Something";
            public string Color { get; } = "#FFFFFF";
            public string Value { get; } = "19.93";
            public int Percentage { get; } = 42;
        }

        public sealed class TheConstructor : ReportDonutChartLegendItemElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsAllProperties()
            {
                var element = new ReportDonutChartLegendItemElement(Name, Color, Value, Percentage);

                element.Name.Should().Be(Name);
                element.Value.Should().Be(Value);
                element.Color.Should().Be(Color);
                element.Percentage.Should().Be(Percentage);
            }
        }

        public sealed class TheEqualsMethod : ReportDonutChartLegendItemElementsBaseTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var elementA = new ReportDonutChartLegendItemElement(Name, Color, Value, Percentage);
                var elementB = new ReportDonutChartLegendItemElement(Name, Color, Value, Percentage);

                elementA.Equals(elementB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInName()
            {
                var nameA = "A";
                var nameB = "B";

                var elementA = new ReportDonutChartLegendItemElement(nameA, Color, Value, Percentage);
                var elementB = new ReportDonutChartLegendItemElement(nameB, Color, Value, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInColor()
            {
                var white = "#FFFFFF";
                var black = "#000000";

                var elementA = new ReportDonutChartLegendItemElement(Name, white, Value, Percentage);
                var elementB = new ReportDonutChartLegendItemElement(Name, black, Value, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInValue()
            {
                var valueA = "1";
                var valueB = "2";

                var elementA = new ReportDonutChartLegendItemElement(Name, Color, valueA, Percentage);
                var elementB = new ReportDonutChartLegendItemElement(Name, Color, valueB, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInPercentage()
            {
                var percentageA = 1;
                var percentageB = 2;

                var elementA = new ReportDonutChartLegendItemElement(Name, Color, Value, percentageA);
                var elementB = new ReportDonutChartLegendItemElement(Name, Color, Value, percentageB);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var legendItemElement = new ReportDonutChartLegendItemElement(Name, Color, Value, Percentage);
                var reportElement = new ReportErrorElement(new Exception());

                legendItemElement.Equals(reportElement).Should().BeFalse();
            }
        }
    }
}
