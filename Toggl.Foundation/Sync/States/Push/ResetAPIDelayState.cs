using System;
using System.Reactive.Linq;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.Sync.States.Push
{
    internal sealed class ResetAPIDelayState : ISyncState
    {
        public StateResult Continue { get; } = new StateResult();

        private readonly IRetryDelayService delay;

        public ResetAPIDelayState(IRetryDelayService delay)
        {
            this.delay = delay;
        }

        public IObservable<ITransition> Start()
            => Observable.Return(Continue.Transition())
                .Do(delay.Reset);
    }
}
