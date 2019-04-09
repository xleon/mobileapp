using System;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Interactors
{
    internal class CreateProjectInteractor : IInteractor<IObservable<IThreadSafeProject>>
    {
        private readonly CreateProjectDTO dto;
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> dataSource;

        public CreateProjectInteractor(
            IIdProvider idProvider, 
            ITimeService timeService,  
            IDataSource<IThreadSafeProject, IDatabaseProject> dataSource,
            CreateProjectDTO dto)
        {
            Ensure.Argument.IsNotNull(dto, nameof(dto));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dto = dto;
            this.dataSource = dataSource;
            this.idProvider = idProvider;
            this.timeService = timeService;
        }

        public IObservable<IThreadSafeProject> Execute()
            => idProvider.GetNextIdentifier()
                .Apply(Project.Builder.Create)
                .SetName(dto.Name)
                .SetColor(dto.Color)
                .SetClientId(dto.ClientId)
                .SetBillable(dto.Billable)
                .SetWorkspaceId(dto.WorkspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .Build()
                .Apply(dataSource.Create);
    }
}