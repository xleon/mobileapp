using FluentAssertions;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Xunit;
using static Toggl.Core.UI.ViewModels.Reports.ReportErrorElement;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class ReportErrorElementTests
    {
        public sealed class TheReportErrorElement
        {
            [Theory, LogIfTooSlow]
            [InlineData(typeof(InvalidOperationException), ErrorType.DataError, true)]
            [InlineData(typeof(ArgumentOutOfRangeException), ErrorType.DataError, true)]
            [InlineData(typeof(ArgumentNullException), ErrorType.DataError, true)]
            [InlineData(typeof(DivideByZeroException), ErrorType.DataError, true)]
            [InlineData(typeof(OfflineException), ErrorType.ConnectionError, false)]
            public void InitializesMessageAndTypeBasedOnExceptionType(Type type, ErrorType errorType, bool genericMessage)
            {
                var exception = Activator.CreateInstance(type) as Exception;
                var errorElement = new ReportErrorElement(exception);

                errorElement.Type.Should().Be(errorType);
                errorElement.Message.Should().Be(genericMessage ? Resources.ReportErrorGeneric : Resources.ReportErrorOffline);
            }
        }

        public sealed class TheEqualsMethod
        {
            [Theory, LogIfTooSlow]
            [InlineData(typeof(InvalidOperationException), typeof(InvalidOperationException), true)]
            [InlineData(typeof(OfflineException), typeof(OfflineException), true)]
            [InlineData(typeof(ArgumentNullException), typeof(InvalidOperationException), true)]
            [InlineData(typeof(OfflineException), typeof(InvalidOperationException), false)]
            public void ReturnsTrueForSameTypeOfError(Type typeA, Type typeB, bool expectedAreEqual)
            {
                var errorA = Activator.CreateInstance(typeA) as Exception;
                var errorB = Activator.CreateInstance(typeB) as Exception;

                var errorElementA = new ReportErrorElement(errorA);
                var errorElementB = new ReportErrorElement(errorB);

                errorElementA.Equals(errorElementB).Should().Be(expectedAreEqual);
            }
        }
    }
}
