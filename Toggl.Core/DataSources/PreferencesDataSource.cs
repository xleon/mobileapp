using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class PreferencesDataSource
        : SingletonDataSource<IThreadSafePreferences, IDatabasePreferences>
    {
        public PreferencesDataSource(ISingleObjectStorage<IDatabasePreferences> storage)
            : base(storage, Preferences.DefaultPreferences)
        {
        }

        protected override IThreadSafePreferences Convert(IDatabasePreferences entity)
            => Preferences.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabasePreferences first, IDatabasePreferences second)
            => Resolver.ForPreferences.Resolve(first, second);
    }
}
