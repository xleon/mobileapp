using System;
using FluentAssertions;
using Toggl.Foundation.Diagnostics;
using Xunit;

namespace Toggl.Foundation.Tests.Diagnostics
{
    public sealed class BaseStopwatchFactoryTests
    {
        public sealed class TheCreateMethod
        {
            private readonly BaseStopwatchFactory stopwatchFactory = new TestStopwatchFactory();

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ReturnsStopwatchesThatCanOnlyBeStartedOnce(bool outputToConsole)
            {
                var stopwatch = stopwatchFactory.Create(MeasuredOperation.None, outputToConsole);
                stopwatch.Start();

                Action callingStartTwice = stopwatch.Start;

                callingStartTwice.Should().Throw<InvalidOperationException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ReturnsStopwatchesThatCanOnlyBeStoppedOnceStarted(bool outputToConsole)
            {
                var stopwatch = stopwatchFactory.Create(MeasuredOperation.None, outputToConsole);

                Action callingStopTwice = stopwatch.Stop;

                callingStopTwice.Should().Throw<InvalidOperationException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public void ReturnsStopwatchesThatCanOnlyBeStoppedOnce(bool outputToConsole)
            {
                var stopwatch = stopwatchFactory.Create(MeasuredOperation.None, outputToConsole);
                stopwatch.Start();
                stopwatch.Stop();

                Action callingStopBeforeStart = stopwatch.Stop;

                callingStopBeforeStart.Should().Throw<InvalidOperationException>();
            }

            private class TestStopwatchFactory : BaseStopwatchFactory
            {
                protected override IStopwatch NativeCreate(MeasuredOperation operation)
                    => new DummyStopwatch(operation);

                private class DummyStopwatch : IStopwatch
                {
                    public MeasuredOperation Operation { get; }

                    public DummyStopwatch(MeasuredOperation operation)
                    {
                        Operation = operation;
                    }

                    public void Start()
                    {
                    }

                    public void Stop()
                    {
                    }
                }
            }
        }
    }
}