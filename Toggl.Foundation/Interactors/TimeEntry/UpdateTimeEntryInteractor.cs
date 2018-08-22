using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Extensions;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;

namespace Toggl.Foundation.Interactors
{
    public sealed class UpdateTimeEntryInteractor : IInteractor<IObservable<IThreadSafeTimeEntry>>
    {
        private readonly ITogglDataSource dataSource;
        private readonly DTOs.EditTimeEntryDto dto;

        public UpdateTimeEntryInteractor(ITogglDataSource dataSource, DTOs.EditTimeEntryDto dto)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(dto, nameof(dto));

            this.dataSource = dataSource;
            this.dto = dto;
        }

        public IObservable<IThreadSafeTimeEntry> Execute()
            => dataSource
                .TimeEntries
                .Update(dto)
                .Do(dataSource.SyncManager.InitiatePushSync);
    }
}
