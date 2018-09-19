using System;
using System.Reactive.Linq;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Interactors
{
    internal class UpdatePreferencesInteractor : IInteractor<IObservable<IThreadSafePreferences>>
    {
        private readonly EditPreferencesDTO dto;
        private readonly ISingletonDataSource<IThreadSafePreferences> dataSource;

        public UpdatePreferencesInteractor(ISingletonDataSource<IThreadSafePreferences> dataSource, EditPreferencesDTO dto)
        {
            Ensure.Argument.IsNotNull(dto, nameof(dto));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dto = dto;
            this.dataSource = dataSource;
        }

        public IObservable<IThreadSafePreferences> Execute()
            => dataSource.Get()
               .Select(updatedPreferences)
               .SelectMany(dataSource.Update);

        private IThreadSafePreferences updatedPreferences(IThreadSafePreferences existing)
            => existing.With(
                dateFormat: dto.DateFormat,
                durationFormat: dto.DurationFormat,
                timeOfDayFormat: dto.TimeOfDayFormat,
                collapseTimeEntries: dto.CollapseTimeEntries,
                syncStatus: SyncStatus.SyncNeeded
            );
    }
}