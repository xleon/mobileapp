using System;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Interactors
{
    internal class CreateTagInteractor : IInteractor<IObservable<IThreadSafeTag>>
    {
        private readonly string tagName;
        private readonly long workspaceId;
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IDataSource<IThreadSafeTag, IDatabaseTag> dataSource;

        public CreateTagInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            IDataSource<IThreadSafeTag, IDatabaseTag> dataSource,
            string tagName,
            long workspaceId)
        {
            Ensure.Argument.IsNotNull(tagName, nameof(tagName));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotZero(workspaceId, nameof(workspaceId));

            this.tagName = tagName;
            this.dataSource = dataSource;
            this.idProvider = idProvider;
            this.timeService = timeService;
            this.workspaceId = workspaceId;
        }

        public IObservable<IThreadSafeTag> Execute()
            => idProvider
                .GetNextIdentifier()
                .Apply(Tag.Builder.Create)
                .SetName(tagName)
                .SetWorkspaceId(workspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build()
                .Apply(dataSource.Create);
    }
}