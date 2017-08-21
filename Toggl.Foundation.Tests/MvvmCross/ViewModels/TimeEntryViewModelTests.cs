using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class TimeEntryViewModelTests
    {
        public class TimeEntryViewModelTest : BaseMvvmCrossTests
        {
            protected IDatabaseProject Project = Substitute.For<IDatabaseProject>();
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected IDatabaseTimeEntry MockTimeEntry = Substitute.For<IDatabaseTimeEntry>();

            protected Subject<DateTimeOffset> TickSubject = new Subject<DateTimeOffset>();

            protected TimeEntryViewModelTest()
            {
                var observable = TickSubject.AsObservable().Publish();
                observable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(observable);
            }
        }

        public class TheConstructor : TimeEntryViewModelTest
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntryViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheHasProjectProperty : TimeEntryViewModelTest
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void ChecksIfTheTimeEntryProvidedHasANonNullProject(bool hasProject)
            {
                MockTimeEntry.Stop.Returns(DateTimeOffset.UtcNow);
                MockTimeEntry.Project.Returns(hasProject ? Project : null);

                var viewModel = new TimeEntryViewModel(MockTimeEntry);

                viewModel.HasProject.Should().Be(hasProject);
            }
        }
    }
}
