using System;
using System.Collections.Generic;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Interactors
{
    internal class GetAllClientsInWorkspaceInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeClient>>>
    {
        private readonly long workspaceId;
        private readonly IDataSource<IThreadSafeClient, IDatabaseClient> dataSource;

        public GetAllClientsInWorkspaceInteractor(IDataSource<IThreadSafeClient, IDatabaseClient> dataSource, long workspaceId)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(workspaceId, nameof(workspaceId));

            this.dataSource = dataSource;
            this.workspaceId = workspaceId;
        }

        public IObservable<IEnumerable<IThreadSafeClient>> Execute()
            => dataSource.GetAll(c => c.WorkspaceId == workspaceId);
    }
}