using System;
using System.Reactive.Linq;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public sealed class PreferencesDataSource : IPreferencesSource
    {
        private ISingleObjectStorage<IDatabasePreferences> databaseStorage;

        public PreferencesDataSource(ISingleObjectStorage<IDatabasePreferences> databaseStorage)
        {
            Ensure.Argument.IsNotNull(databaseStorage, nameof(databaseStorage));

            this.databaseStorage = databaseStorage;
        }

        public IObservable<IDatabasePreferences> Get()
            => databaseStorage.Single().Select(Preferences.From);

        public IObservable<IDatabasePreferences> Update(EditPreferencesDTO dto)
            => databaseStorage
                .Single()
                .Select(preferences => updatedPreferences(preferences, dto))
                .SelectMany(databaseStorage.Update)
                .Select(Preferences.From);

        private IDatabasePreferences updatedPreferences(IDatabasePreferences existing, EditPreferencesDTO dto)
            => Preferences.Builder
                .FromExisting(existing)
                .SetDateFormat(dto.DateFormat)
                .Build();
    }
}
