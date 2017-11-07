using System;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Xunit;

namespace Toggl.Foundation.Tests
{
    public sealed class TimeServiceTests
    {
        public sealed class TheCurrentTimeObservableProperty
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

            [Fact]
            public void PublishesCurrentTimeFlooredToTheCurrentSecond()
            {
                DateTimeOffset roundedNow = default(DateTimeOffset);
                timeService.CurrentDateTimeObservable.Subscribe(time => roundedNow = time);

                testScheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);

                roundedNow.Should().NotBe(default(DateTimeOffset));
                roundedNow.Millisecond.Should().Be(0);
            }

            [Fact]
            public void ReturnsCurrentTimeFlooredToTheCurrentSecond()
            {
                DateTimeOffset roundedNow = timeService.CurrentDateTime;

                roundedNow.Millisecond.Should().Be(0);
            }
        }
    }
}
