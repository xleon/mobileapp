using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Interactors
{
    internal sealed class GetDefaultWorkspaceInteractor : IInteractor<IObservable<IDatabaseWorkspace>>
    {
        private readonly ITogglDatabase database;

        public GetDefaultWorkspaceInteractor(ITogglDatabase database)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));

            this.database = database;
        }

        public IObservable<IDatabaseWorkspace> Execute()
            => database.User
                .Single()
                .SelectMany(user => user.DefaultWorkspaceId.HasValue
                    ? database.Workspaces.GetById(user.DefaultWorkspaceId.Value)
                    : chooseWorkspace())
                .Catch((InvalidOperationException exception) => chooseWorkspace())
                .Select(Workspace.From);

        private IObservable<IDatabaseWorkspace> chooseWorkspace()
            => database.Workspaces.GetAll(workspace => !workspace.IsDeleted)
                .Select(workspaces => workspaces.OrderBy(workspace => workspace.Id).First());
    }
}
