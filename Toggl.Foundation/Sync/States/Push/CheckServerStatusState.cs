using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.Sync.States.Push
{
    internal sealed class CheckServerStatusState
    {
        public StateResult Retry { get; } = new StateResult();
        public StateResult ServerIsAvailable { get; } = new StateResult();

        private ITogglApi api;
        private IScheduler scheduler;
        private IRetryDelayService apiDelay;
        private IRetryDelayService statusDelay;

        public CheckServerStatusState(ITogglApi api, IScheduler scheduler, IRetryDelayService apiDelay, IRetryDelayService statusDelay)
        {
            this.api = api;
            this.scheduler = scheduler;
            this.apiDelay = apiDelay;
            this.statusDelay = statusDelay;
        }

        public IObservable<ITransition> Start()
            => api.Status.IsAvailable()
                .Delay(apiDelay.NextSlowDelay(), scheduler)
                .Do(_ => statusDelay.Reset())
                .SelectMany(proceed)
                .Catch((Exception e) => delayedRetry(getDelay(e)));

        private IObservable<ITransition> proceed
            => Observable.Return(ServerIsAvailable.Transition());

        private IObservable<ITransition> delayedRetry(TimeSpan period)
            => Observable.Return(Retry.Transition()).Delay(period, scheduler);

        private TimeSpan getDelay(Exception exception)
            => exception is InternalServerErrorException
                ? statusDelay.NextSlowDelay()
                : statusDelay.NextFastDelay();
    }
}
