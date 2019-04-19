using System;

namespace Toggl.Core.Services
{
    public interface IBackgroundService
    {
        void EnterBackground();
        void EnterForeground();

        IObservable<TimeSpan> AppResumedFromBackground { get; }
    }
}
