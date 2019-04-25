using System;
using System.Reactive.Concurrency;
using Android.OS;
using Toggl.Core.Analytics;
using Toggl.Shared;

namespace Toggl.Droid
{
    public sealed class AndroidSchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainScheduler { get; }
        public IScheduler DefaultScheduler { get; }
        public IScheduler BackgroundScheduler { get; }

        public AndroidSchedulerProvider(IAnalyticsService analyticsService)
        {
            MainScheduler = new HandlerScheduler(new Handler(Looper.MainLooper), Looper.MainLooper.Thread.Id, analyticsService);
            DefaultScheduler = new TrackedSchedulerWrapper(Scheduler.Default, analyticsService);
            BackgroundScheduler = new TrackedSchedulerWrapper(NewThreadScheduler.Default, analyticsService);
        }
        
        public sealed class TrackedSchedulerWrapper : IScheduler
        {
            private readonly string analyticsEventName;
            private readonly IAnalyticsService analyticsService;
            private readonly IScheduler innerScheduler;

            public TrackedSchedulerWrapper(IScheduler innerScheduler, IAnalyticsService analyticsService)
            {
                this.innerScheduler = innerScheduler;
                this.analyticsService = analyticsService;
                analyticsEventName = this.innerScheduler.GetType().Name;
            }

            public DateTimeOffset Now => innerScheduler.Now;

            public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
            {
                try
                {
                    return innerScheduler.Schedule(state, action);
                }
                catch (Exception exception)
                {
                    analyticsService.DebugScheduleError.Track(analyticsEventName, "Schedule:1", exception.GetType().Name, exception.StackTrace);
                    analyticsService.Track(exception, exception.Message);
                    throw;
                }
            }

            public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                try
                {
                    return innerScheduler.Schedule(state, dueTime, action);
                }
                catch (Exception exception)
                {
                    analyticsService.DebugScheduleError.Track(analyticsEventName, "Schedule:2", exception.GetType().Name, exception.StackTrace);
                    analyticsService.Track(exception, exception.Message);
                    throw;
                }
            }

            public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
            {
                try
                {
                    return innerScheduler.Schedule(state, dueTime, action);
                }
                catch (Exception exception)
                {
                    analyticsService.DebugScheduleError.Track(analyticsEventName, "Schedule:3", exception.GetType().Name, exception.StackTrace);
                    analyticsService.Track(exception, exception.Message);
                    throw;
                }
            }
        }
    }
}
