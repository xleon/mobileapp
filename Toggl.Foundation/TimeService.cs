using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toggl.Foundation
{
    public class TimeService : ITimeService
    {
        private readonly IDisposable disposable;

        public DateTimeOffset CurrentDateTime => DateTimeOffset.UtcNow;

        public IConnectableObservable<DateTimeOffset> CurrentDateTimeObservable { get; }

        public TimeService(IScheduler scheduler)
        {
            CurrentDateTimeObservable =
                Observable
                    .Interval(TimeSpan.FromSeconds(1), scheduler)
                    .Select(_ => DateTimeOffset.UtcNow)
                    .Publish();

            disposable = CurrentDateTimeObservable.Connect();
        }
    }
}
