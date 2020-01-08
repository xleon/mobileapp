using FluentAssertions;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportNoDataElementTests
    {
        public sealed class TheEqualsMethod
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForSameType()
            {
                var noDataElementA = new ReportNoDataElement();
                var noDataElementB = new ReportNoDataElement();

                noDataElementA.Equals(noDataElementB).Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForDifferentType()
            {
                var noDataElementA = new ReportNoDataElement();
                var errorElement = new ReportErrorElement(new Exception());

                noDataElementA.Equals(errorElement).Should().BeFalse();
            }
        }
    }
}
