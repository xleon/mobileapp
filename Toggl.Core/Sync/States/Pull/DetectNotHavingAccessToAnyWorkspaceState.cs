using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.DataSources;

namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class DetectNotHavingAccessToAnyWorkspaceState : ISyncState<IFetchObservables>
    {
        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        private readonly ITogglDataSource dataSource;

        public DetectNotHavingAccessToAnyWorkspaceState(ITogglDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => dataSource.Workspaces.GetAll()
                .Select(workspaces => workspaces.Any()
                    ? Done.Transition(fetch)
                    : throw new NoWorkspaceException());
    }
}
