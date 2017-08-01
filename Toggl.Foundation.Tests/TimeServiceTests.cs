using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Toggl.Foundation.Tests
{
    public class TimeServiceTests
    {
        public class TheCurrentTimeObservableProperty
        {
            private readonly TimeService timeService;
            private readonly TestScheduler testScheduler = new TestScheduler();

            public TheCurrentTimeObservableProperty()
            {
                timeService = new TimeService(testScheduler);
            }

            [Fact]
            public void ReturnsTheSameObjectsRegardlessOfTheTimeTheObserversSubscribed()
            {
                var firstStep = 500;
                var secondStep = 3500;
                var expectedNumberOfMessages = (firstStep + secondStep) / 1000;
                var firstObserver = testScheduler.CreateObserver<DateTimeOffset>();
                var secondObserver = testScheduler.CreateObserver<DateTimeOffset>();

                timeService.CurrentDateTimeObservable.Subscribe(firstObserver);
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(firstStep).Ticks);
                timeService.CurrentDateTimeObservable.Subscribe(secondObserver);
                testScheduler.AdvanceBy(TimeSpan.FromMilliseconds(secondStep).Ticks);

                secondObserver.Messages
                             .Should().HaveCount(expectedNumberOfMessages)
                             .And.BeEquivalentTo(firstObserver.Messages);
            }
        }
    }
}
