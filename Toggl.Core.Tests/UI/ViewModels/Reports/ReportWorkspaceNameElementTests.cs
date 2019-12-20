using FluentAssertions;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportWorkspaceNameElementTests
    {
        private const string workspace = "WorkspaceA";
        private const string otherworkspace = "WorkspaceB";

        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void SetsTheCorrectName()
            {
                var element = new ReportWorkspaceNameElement(workspace);
                element.Name.Should().Be(workspace);
            }
        }
        public sealed class TheEqualsMethod
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForEqualElements()
            {
                var elementA = new ReportWorkspaceNameElement(workspace);
                var elementB = new ReportWorkspaceNameElement(workspace);

                elementA.Equals(elementB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInName()
            {
                var elementA = new ReportWorkspaceNameElement(workspace);
                var elementB = new ReportWorkspaceNameElement(otherworkspace);

                elementA.Equals(elementB).Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForElementsThatDifferInType()
            {
                var elementA = new ReportWorkspaceNameElement(workspace);
                var elementB = new ReportNoDataElement();

                elementA.Equals(elementB).Should().BeFalse();
            }
        }

    }
}
