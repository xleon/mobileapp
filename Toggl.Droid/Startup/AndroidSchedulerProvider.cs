using System.Reactive.Concurrency;
using Android.OS;
using Toggl.Shared;

namespace Toggl.Droid
{
    public sealed class AndroidSchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainScheduler { get; }
        public IScheduler DefaultScheduler { get; }
        public IScheduler BackgroundScheduler { get; }

        public AndroidSchedulerProvider()
        {
            MainScheduler = new HandlerScheduler(new Handler(Looper.MainLooper), Looper.MainLooper.Thread.Id);
            DefaultScheduler = Scheduler.Default;
            BackgroundScheduler = NewThreadScheduler.Default;
        }
    }
}
