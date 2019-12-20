using FluentAssertions;
using FsCheck.Xunit;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportProjectsDonutChartLegendItemElementTests
    {
        public class ReportProjectsDonutChartLegendItemElementBaseTests
        {
            public string Name { get; } = "Something";
            public string Color { get; } = "#FFFFFF";
            public string Client { get; } = "Someone";
            public string Value { get; } = "19.93";
            public double Percentage { get; } = 42;
        }

        public sealed class TheConstructor : ReportProjectsDonutChartLegendItemElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void SetsAllProperties()
            {
                var element = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, Percentage);

                element.Name.Should().Be(Name);
                element.Client.Should().Be(Client);
                element.Value.Should().Be(Value);
                element.Color.Should().Be(Color);
                element.Percentage.Should().Be(Percentage);
            }
        }

        public sealed class TheEqualsMethod : ReportProjectsDonutChartLegendItemElementBaseTests
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var elementA = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, Percentage);
                var elementB = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, Percentage);

                elementA.Equals(elementB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInName()
            {
                var nameA = "A";
                var nameB = "B";

                var elementA = new ReportProjectsDonutChartLegendItemElement(nameA, Color, Client, Value, Percentage);
                var elementB = new ReportProjectsDonutChartLegendItemElement(nameB, Color, Client, Value, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInClient()
            {
                var someone = "Someone";
                var anyone = "Anyone";

                var elementA = new ReportProjectsDonutChartLegendItemElement(Name, Color, someone, Value, Percentage);
                var elementB = new ReportProjectsDonutChartLegendItemElement(Name, Color, anyone, Value, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }


            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInColor()
            {
                var white = "#FFFFFF";
                var black = "#000000";

                var elementA = new ReportProjectsDonutChartLegendItemElement(Name, white, Client, Value, Percentage);
                var elementB = new ReportProjectsDonutChartLegendItemElement(Name, black, Client, Value, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInValue()
            {
                var valueA = "1";
                var valueB = "2";

                var elementA = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, valueA, Percentage);
                var elementB = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, valueB, Percentage);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInPercentage()
            {
                var percentageA = 1;
                var percentageB = 2;

                var elementA = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, percentageA);
                var elementB = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, percentageB);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var legendItemElement = new ReportProjectsDonutChartLegendItemElement(Name, Color, Client, Value, Percentage);
                var reportElement = new ReportErrorElement(new Exception());

                legendItemElement.Equals(reportElement).Should().BeFalse();
            }
        }
    }
}
