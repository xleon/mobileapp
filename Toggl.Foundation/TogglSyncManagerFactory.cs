using System;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using System.Reactive.Concurrency;

namespace Toggl.Foundation
{
    public static class TogglSyncManager
    {
        public static ISyncManager CreateSyncManager(ITogglDatabase database, ITogglApi api, IScheduler scheduler)
        {
            var queue = new SyncStateQueue();
            var entryPoints = new StateMachineEntryPoints();
            var transitions = new TransitionHandlerProvider();
            ConfigureTransitions(transitions, database, api, entryPoints);
            var stateMachine = new StateMachine(transitions, scheduler);
            var orchestrator = new StateMachineOrchestrator(stateMachine, entryPoints);

            return new SyncManager(queue, orchestrator);
        }

        public static void ConfigureTransitions(TransitionHandlerProvider transitions, ITogglDatabase database, ITogglApi api, StateMachineEntryPoints entryPoints)
        {
            configurePullTransitions(transitions, database, api, entryPoints.StartPullSync);
        }

        private static void configurePullTransitions(TransitionHandlerProvider transitions, ITogglDatabase database, ITogglApi api, StateResult entryPoint)
        {
            var fetchAllSince = new FetchAllSinceState(database, api);
            var persistWorkspaces = new PersistWorkspacesState(database);
            var persistTags = new PersistTagsState(database);
            var persistClients = new PersistClientsState(database);
            var persistProjects = new PersistProjectsState(database);
            var persistTimeEntries = new PersistTimeEntriesState(database);

            transitions.ConfigureTransition(entryPoint, fetchAllSince.Start);
            transitions.ConfigureTransition(fetchAllSince.FetchStarted, persistWorkspaces.Start);
            transitions.ConfigureTransition(persistWorkspaces.FinishedPersisting, persistTags.Start);
            transitions.ConfigureTransition(persistTags.FinishedPersisting, persistClients.Start);
            transitions.ConfigureTransition(persistClients.FinishedPersisting, persistProjects.Start);
            transitions.ConfigureTransition(persistProjects.FinishedPersisting, persistTimeEntries.Start);
        }
    }
}
