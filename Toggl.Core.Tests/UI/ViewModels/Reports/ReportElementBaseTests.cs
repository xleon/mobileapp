using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportElementBaseTests
    {
        private class MockElement : ReportElementBase
        {
            public MockElement(bool isLoading) : base(isLoading) { }

            public override bool Equals(IReportElement other)
                => other is MockElement;
        }

        public sealed class TheIsLoadingProperty
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void IsSetFromTheConstructorProvidedValue(bool isLoading)
            {
                var mockElement = new MockElement(isLoading);
                mockElement.IsLoading.Should().Be(isLoading);
            }
        }
    }
}
