using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;

namespace Toggl.Foundation.Interactors
{
    internal class UpdateWorkspaceInteractor : IInteractor<IObservable<IThreadSafeUser>>
    {
        private ISingletonDataSource<IThreadSafeUser> dataSource;
        private long selectedWorkspaceId;

        public UpdateWorkspaceInteractor(ISingletonDataSource<IThreadSafeUser> dataSource, long selectedWorkspaceId)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(selectedWorkspaceId, nameof(selectedWorkspaceId));

            this.dataSource = dataSource;
            this.selectedWorkspaceId = selectedWorkspaceId;
        }

        public IObservable<IThreadSafeUser> Execute()
            => dataSource.Get()
                .Select(user => user.With(selectedWorkspaceId))
                .SelectMany(dataSource.Update);
    }
}