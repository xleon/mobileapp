using System;
using Toggl.Foundation.Sync.ConflictResolution.Selectors;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Sync.ConflictResolution
{
    internal static class Resolver
    {
        public static PreferNewer<IDatabaseClient> ForClients()
            => new PreferNewer<IDatabaseClient>(new ClientSyncSelector());

        public static PreferNewer<IDatabaseProject> ForProjects()
            => new PreferNewer<IDatabaseProject>(new ProjectSyncSelector());

        public static PreferNewer<IDatabaseWorkspace> ForWorkspaces()
            => new PreferNewer<IDatabaseWorkspace>(new WorkspaceSyncSelector());

        public static PreferNewer<IDatabaseTask> ForTasks()
            => new PreferNewer<IDatabaseTask>(new TaskSyncSelector());

        public static PreferNewer<IDatabaseTag> ForTags()
            => new PreferNewer<IDatabaseTag>(new TagSyncSelector());

        public static PreferNewer<IDatabaseTimeEntry> ForTimeEntries()
            => new PreferNewer<IDatabaseTimeEntry>(new TimeEntrySyncSelector(), TimeSpan.FromSeconds(5));
    }
}
