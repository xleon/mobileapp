using Android.OS;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Toggl.Droid
{
    /// <summary>
    /// Custom handler scheduler
    /// </summary>
    /// <remarks>
    /// Roughly based on reactiveui/ReactiveUI HandlerScheduler class
    /// See https://github.com/reactiveui/ReactiveUI/blob/master/src/ReactiveUI/Platforms/android/HandlerScheduler.cs
    /// </remarks>
    public sealed class HandlerScheduler : IScheduler
    {
        private readonly Handler handler;

        public DateTimeOffset Now => DateTimeOffset.Now;

        public HandlerScheduler(Handler handler)
        {
            this.handler = handler;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            bool isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

            handler.Post(() => 
            {
                if (isCancelled)
                    return;

                innerDisp.Disposable = action(this, state);
            });

            return new CompositeDisposable(
                Disposable.Create(() => isCancelled = true),
                innerDisp
            );
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var isCancelled = false;
            var innerDisp = new SerialDisposable { Disposable = Disposable.Empty };

                handler.PostDelayed(() => 
                {
                    if (isCancelled)
                        return;

                    innerDisp.Disposable = action(this, state);
                }, dueTime.Ticks / TimeSpan.TicksPerMillisecond);

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
