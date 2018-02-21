using System;
using System.Reactive.Linq;
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

        public IObservable<IDatabasePreferences> Update(IDatabasePreferences preferences)
            => databaseStorage.Update(preferences).Select(Preferences.From);
    }
}
