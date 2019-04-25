using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Android.OS;
using Toggl.Core.Analytics;
using Toggl.Droid.Extensions;

namespace Toggl.Droid
{
    public sealed class HandlerScheduler : IScheduler
    {        
        private readonly long looperId;
        private readonly Handler handler;
        private IAnalyticsService analyticsService;

        public DateTimeOffset Now => DateTimeOffset.Now;

        public HandlerScheduler(Handler handler, long? threadIdAssociatedWithHandler, IAnalyticsService analyticsService)
        {
            this.handler = handler;
            this.analyticsService = analyticsService;
            looperId = threadIdAssociatedWithHandler ?? -1;
        }
        
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            bool isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

            try
            {
                if (looperId > 0 && looperId == Java.Lang.Thread.CurrentThread().Id)
                    return action(this, state);
            }
            catch (Exception exception)
            {
                analyticsService.DebugScheduleError.Track(nameof(HandlerScheduler), "Schedule:1:1", exception.GetType().Name, exception.StackTrace);
                analyticsService.Track(exception, exception.Message);
                throw;
            }

            try
            {
                handler.Post(() => 
                {
                    if (isCancelled)
                        return;

                    try
                    {
                        innerDisp.Disposable = action(this, state);
                    }
                    catch (Exception exception)
                    {
                        analyticsService.DebugScheduleError.Track(nameof(HandlerScheduler), "Schedule:1:2", exception.GetType().Name, exception.StackTrace);
                        analyticsService.Track(exception, exception.Message);
                        throw;
                    }                    
                });    
            }
            catch (Exception exception)
            {
                analyticsService.DebugScheduleError.Track(nameof(HandlerScheduler), "Schedule:1:3", exception.GetType().Name, exception.StackTrace);
                analyticsService.Track(exception, exception.Message);
                throw;
            }

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp
            );
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

            try
            {
                handler.PostDelayed(() => 
                    {
                        if (isCancelled)
                            return;

                        try
                        {
                            innerDisp.Disposable = action(this, state);
                        }
                        catch (Exception exception)
                        {
                            analyticsService.DebugScheduleError.Track(nameof(HandlerScheduler), "Schedule:2:1", exception.GetType().Name, exception.StackTrace);
                            analyticsService.Track(exception, exception.Message);
                            throw;
                        }
                    }, dueTime.Ticks / TimeSpan.TicksPerMillisecond);
            }
            catch (Exception exception)
            {
                analyticsService.DebugScheduleError.Track(nameof(HandlerScheduler), "Schedule:2:2", exception.GetType().Name, exception.StackTrace);
                analyticsService.Track(exception, exception.Message);
                throw;
            }
            
            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp
            );
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Now)
                return Schedule(state, action);

            return Schedule(state, dueTime - Now, action);
        }
    }
}
