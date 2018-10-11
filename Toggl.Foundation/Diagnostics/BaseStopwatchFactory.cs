using System;
using System.Diagnostics;

namespace Toggl.Foundation.Diagnostics
{
    public abstract class BaseStopwatchFactory : IStopwatchFactory
    {
        public IStopwatch Create(MeasuredOperation operation, bool outputToConsole = false)
        {
            var stopwatch = NativeCreate(operation);

            return outputToConsole 
                ? new ConsoleStopwatch(stopwatch)
                : new SingleUseStopwatch(stopwatch);
        }

        protected abstract IStopwatch NativeCreate(MeasuredOperation operation);

        private class SingleUseStopwatch : IStopwatch
        {
            private bool didStop;
            private bool didStart;

            public MeasuredOperation Operation { get; }

            protected IStopwatch InnerStopwatch { get; }

            public SingleUseStopwatch(IStopwatch innerStopwatch)
            {
                InnerStopwatch = innerStopwatch;
                Operation = innerStopwatch.Operation;
            }

            public virtual void Start()
            {
                if (didStart)
                    throw new InvalidOperationException("You can not start a stopwatch more than once");

                didStart = true;

                InnerStopwatch.Start();
            }

            public virtual void Stop()
            {
                if (!didStart)
                    throw new InvalidOperationException("You need to start a stopwatch before stopping it");

                if (didStop)
                    throw new InvalidOperationException("You can not stop a stopwatch more than once");

                didStop = true;

                InnerStopwatch.Stop();
            }
        }

        private class ConsoleStopwatch : SingleUseStopwatch
        {
            private readonly Stopwatch stopwatch = new Stopwatch();

            public ConsoleStopwatch(IStopwatch innerStopwatch)
                : base(innerStopwatch) { }

            public override void Start()
            {
                stopwatch.Start();
                base.Start();
            }

            public override void Stop()
            {
                base.Stop();
                stopwatch.Stop();

                Console.WriteLine($"Operation {Operation} took {stopwatch.Elapsed} to complete");
            }
        }
    }
}
