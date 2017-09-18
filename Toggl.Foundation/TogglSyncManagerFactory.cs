using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using System.Reactive.Concurrency;
using Toggl.Foundation.Tests.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;

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
            configurePushTransitions(transitions, database, api, entryPoints.StartPushSync);
        }

        private static void configurePullTransitions(TransitionHandlerProvider transitions, ITogglDatabase database, ITogglApi api, StateResult entryPoint)
        {
            var fetchAllSince = new FetchAllSinceState(database, api);
            var persistWorkspaces = new PersistWorkspacesState(database.Workspaces, database.SinceParameters);
            var persistTags = new PersistTagsState(database.Tags, database.SinceParameters);
            var persistClients = new PersistClientsState(database.Clients, database.SinceParameters);
            var persistProjects = new PersistProjectsState(database.Projects, database.SinceParameters);
            var persistTimeEntries = new PersistTimeEntriesState(database.TimeEntries, database.SinceParameters);

            transitions.ConfigureTransition(entryPoint, fetchAllSince.Start);
            transitions.ConfigureTransition(fetchAllSince.FetchStarted, persistWorkspaces.Start);
            transitions.ConfigureTransition(persistWorkspaces.FinishedPersisting, persistTags.Start);
            transitions.ConfigureTransition(persistTags.FinishedPersisting, persistClients.Start);
            transitions.ConfigureTransition(persistClients.FinishedPersisting, persistProjects.Start);
            transitions.ConfigureTransition(persistProjects.FinishedPersisting, persistTimeEntries.Start);
        }
        
        private static void configurePushTransitions(TransitionHandlerProvider transitions, ITogglDatabase database, ITogglApi api, StateResult entryPoint)
        {
            configurePushTransitionsForTimeEntries(transitions, database, api, entryPoint);
        }

        private static IStateResult configurePushTransitionsForTimeEntries(TransitionHandlerProvider transitions, ITogglDatabase database, ITogglApi api, StateResult entryPoint)
        {
            var push = new PushTimeEntriesState(database);
            var pushOne = new PushOneEntityState<IDatabaseTimeEntry>();
            var create = new CreateTimeEntryState(api, database.TimeEntries);
            var update = new UpdateTimeEntryState(api, database.TimeEntries);
            var unsyncable = new UnsyncableTimeEntryState(database.TimeEntries);

            return configurePush(transitions, entryPoint, push, pushOne, create, update, unsyncable);
        }

        private static IStateResult configurePush<T>(
            TransitionHandlerProvider transitions,
            IStateResult entryPoint,
            BasePushState<T> push,
            PushOneEntityState<T> pushOne,
            BaseCreateEntityState<T> create,
            BaseUpdateEntityState<T> update,
            BaseUnsyncableEntityState<T> markUnsyncable)
            where T : class, IBaseModel, IDatabaseSyncable
        {
            transitions.ConfigureTransition(entryPoint, push.Start);
            transitions.ConfigureTransition(push.PushEntity, pushOne.Start);
            transitions.ConfigureTransition(pushOne.CreateEntity, create.Start);
            transitions.ConfigureTransition(pushOne.UpdateEntity, update.Start);
            transitions.ConfigureTransition(create.CreatingFinished, push.Start);
            transitions.ConfigureTransition(create.CreatingFailed, markUnsyncable.Start);
            transitions.ConfigureTransition(update.UpdatingSucceeded, push.Start);
            transitions.ConfigureTransition(update.UpdatingFailed, markUnsyncable.Start);

            return push.NothingToPush;
        }
    }
}
