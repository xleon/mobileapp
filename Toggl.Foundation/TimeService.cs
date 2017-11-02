using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Toggl.Foundation
{
    public sealed class TimeService : ITimeService
    {
        private readonly IDisposable disposable;

        public DateTimeOffset CurrentDateTime => floor(DateTimeOffset.UtcNow);

        public IConnectableObservable<DateTimeOffset> CurrentDateTimeObservable { get; }

        public TimeService(IScheduler scheduler)
        {
            CurrentDateTimeObservable =
                Observable
                    .Interval(TimeSpan.FromSeconds(1), scheduler)
                    .Select(_ => CurrentDateTime)
                    .Publish();

            disposable = CurrentDateTimeObservable.Connect();
        }

        private DateTimeOffset floor(DateTimeOffset t)
            => new DateTimeOffset(t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second, t.Offset);
    }
}
