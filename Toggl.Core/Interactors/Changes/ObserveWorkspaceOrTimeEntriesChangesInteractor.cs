using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Foundation.Interactors.Changes
{
    public class ObserveWorkspaceOrTimeEntriesChangesInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly IInteractorFactory interactorFactory;

        public ObserveWorkspaceOrTimeEntriesChangesInteractor(IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.interactorFactory = interactorFactory;
        }

        public IObservable<Unit> Execute()
            => Observable.CombineLatest(
                interactorFactory.ObserveWorkspacesChanges().Execute(),
                interactorFactory.ObserveTimeEntriesChanges().Execute(),
                (workspaces, timeEntries) => Unit.Default
            );
    }
}
