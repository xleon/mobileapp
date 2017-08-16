using System;
using System.Linq;
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
    public class TimeEntryViewModelCollectionTests
    {
        public abstract class TimeEntryViewModelCollectionTest : BaseMvvmCrossTests
        {
            protected DateTime Noon = DateTime.UtcNow.Date.AddHours(12);
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected Subject<DateTimeOffset> TickSubject = new Subject<DateTimeOffset>();
            protected IGrouping<DateTime, IDatabaseTimeEntry> Grouping = Substitute.For<IGrouping<DateTime, IDatabaseTimeEntry>>();

            protected TimeEntryViewModelCollectionTest()
            {
                var observable = TickSubject.AsObservable().Publish();
                observable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(observable);
            }
        }

        public class TheConstructor : TimeEntryViewModelCollectionTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useGrouping, bool useTimeService)
            {
                var grouping = useGrouping ? Grouping : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntryViewModelCollection(grouping, timeService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheCollection : TimeEntryViewModelCollectionTest
        {
            [Fact]
            public void HasTheSameAmountOfItemsAsThePassedGrouping()
            {
                var grouping =
                    Enumerable.Range(0, 10)
                    .Select(i =>
                    {
                        var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                        timeEntry.Stop.Returns(Noon.AddHours(-i));
                        timeEntry.Start.Returns(Noon.AddHours(-i - 1));
                        return timeEntry;
                    }).GroupBy(x => x.Start.Date).Single();

                var collection = new TimeEntryViewModelCollection(grouping, TimeService);

                collection.Should().HaveCount(10);
            }
        }

        public class TheTotalTimeProperty : TimeEntryViewModelCollectionTest
        {
            [Fact]
            public void EqualsTheSumOfTheDurationOfAllTimeEntries()
            {
                var grouping =
                    Enumerable.Range(0, 10)
                    .Select(i =>
                    {
                        var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                        timeEntry.Stop.Returns(Noon.AddHours(-i));
                        timeEntry.Start.Returns(Noon.AddHours(-i - 1));
                        return timeEntry;
                    }).GroupBy(x => x.Start.Date).Single();

                var collection = new TimeEntryViewModelCollection(grouping, TimeService);

                collection.TotalTime.Should().Be(TimeSpan.FromHours(10));
            }

            [Fact]
            public void ConsidersTheTimeServiceTicksForTheStopTimeForTheCurrentlyRunningTimeEntry()
            {
                //Arrange
                var timeEntries =
                    Enumerable.Range(1, 9)
                    .Select(i =>
                    {
                        var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                        timeEntry.Stop.Returns(Noon.AddHours(-i));
                        timeEntry.Start.Returns(Noon.AddHours(-i - 1));
                        return timeEntry;
                    }).ToList();

                var currentlyRunningTimeEntry = Substitute.For<IDatabaseTimeEntry>();
                currentlyRunningTimeEntry.Stop.Returns(_ => null);
                currentlyRunningTimeEntry.Start.Returns(Noon);
                timeEntries.Add(currentlyRunningTimeEntry);
                var grouping = timeEntries.OrderBy(x => x.Start).GroupBy(x => x.Start.Date).Single();

                //Act
                var collection = new TimeEntryViewModelCollection(grouping, TimeService);
                TickSubject.OnNext(Noon.AddHours(2));

                //Assert
                collection.TotalTime.Should().Be(TimeSpan.FromHours(11));
            }
        }
    }
}
