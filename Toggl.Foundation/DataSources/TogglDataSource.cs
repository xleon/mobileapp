using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Sync;
using Toggl.Multivac;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        private readonly ITogglDatabase database;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        public TogglDataSource(
            ITogglDatabase database,
            ITimeService timeService,
            IAccessRestrictionStorage accessRestrictionStorage,
            Func<ITogglDataSource, ISyncManager> createSyncManager)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            this.database = database;
            this.accessRestrictionStorage = accessRestrictionStorage;

            User = new UserDataSource(database.User);
            Tags = new TagsDataSource(database.IdProvider, database.Tags, timeService);
            Tasks = new TasksDataSource(database.Tasks);
            Workspaces = new WorkspacesDataSource(database);
            Clients = new ClientsDataSource(database.IdProvider, database.Clients, timeService);
            Projects = new ProjectsDataSource(database.IdProvider, database.Projects, timeService);
            TimeEntries = new TimeEntriesDataSource(database.IdProvider, database.TimeEntries, timeService);

            AutocompleteProvider = new AutocompleteProvider(database);
            SyncManager = createSyncManager(this);
        }

        public IUserSource User { get; }
        public ITagsSource Tags { get; }
        public ITasksSource Tasks { get; }
        public IClientsSource Clients { get; }
        public IProjectsSource Projects { get; }
        public IWorkspacesSource Workspaces { get; }
        public ITimeEntriesSource TimeEntries { get; }

        public ISyncManager SyncManager { get; }
        public IAutocompleteProvider AutocompleteProvider { get; }

        public IObservable<bool> HasUnsyncedData()
            => Observable.Merge(
                hasUnsyncedData(database.TimeEntries),
                hasUnsyncedData(database.Projects),
                hasUnsyncedData(database.User),
                hasUnsyncedData(database.Tasks),
                hasUnsyncedData(database.Clients),
                hasUnsyncedData(database.Tags),
                hasUnsyncedData(database.Workspaces))
                .Any(hasUnsynced => hasUnsynced);

        private IObservable<bool> hasUnsyncedData<TModel>(IRepository<TModel> repository)
            where TModel : IDatabaseSyncable
            => repository
                .GetAll(entity => entity.SyncStatus != SyncStatus.InSync)
                .Select(unsynced => unsynced.Any())
                .SingleAsync();

        public IObservable<Unit> Logout()
            => SyncManager.Freeze()
                .FirstAsync()
                .SelectMany(_ => database.Clear())
                .Do(_ => accessRestrictionStorage.ClearUnauthorizedAccess())
                .FirstAsync();
    }
}
