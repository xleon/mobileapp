using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
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
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useTimeEntry, bool useTimeService)
            {
                var timeEntry = useTimeEntry ? MockTimeEntry : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntryViewModel(timeEntry, timeService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheDurationProperty : TimeEntryViewModelTest
        {
            [Fact]
            public void ConsidersTimeServiceTicksForStopDateIfThePassedTimeEntryHasNoStopDate()
            {
                var tickDate = DateTimeOffset.UtcNow;
                var startDate = tickDate.AddHours(-2);
                MockTimeEntry.Start.Returns(startDate);
                MockTimeEntry.Stop.Returns(_ => null);
                MockTimeEntry.Project.Returns(Project);

                var viewModel = new TimeEntryViewModel(MockTimeEntry, TimeService);
                TickSubject.OnNext(tickDate);

                viewModel.Duration.Should().Be(tickDate - startDate);
            }

            [Fact]
            public void IgnoresTicksIfThePassedTimeEntryHasAStopDate()
            {
                var tickDate = DateTimeOffset.UtcNow;
                var stopTime = tickDate.AddHours(-1);
                var startDate = tickDate.AddHours(-2);
                MockTimeEntry.Start.Returns(startDate);
                MockTimeEntry.Stop.Returns(stopTime);
                MockTimeEntry.Project.Returns(Project);

                var viewModel = new TimeEntryViewModel(MockTimeEntry, TimeService);
                TickSubject.OnNext(tickDate);

                viewModel.Duration.Should().Be(stopTime - startDate);
            }
        }

        public class TheHasProjectProperty : TimeEntryViewModelTest
        {
            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public void ChecksIfTheTimeEntryProvidedHasANonNullProject(bool hasProject)
            {
                MockTimeEntry.Project.Returns(hasProject ? Project : null);

                var viewModel = new TimeEntryViewModel(MockTimeEntry, TimeService);

                viewModel.HasProject.Should().Be(hasProject);
            }
        }
    }
}
