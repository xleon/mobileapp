using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Foundation.Sync;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.Exceptions;

namespace Toggl.Foundation.Tests.Sync.States
{
    internal abstract class BaseUnsyncableEntityState<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        private ITogglDatabase database;

        public StateResult<TModel> MarkedAsUnsyncable { get; } = new StateResult<TModel>();

        public BaseUnsyncableEntityState(ITogglDatabase database)
        {
            this.database = database;
        }

        public IObservable<ITransition> Start((Exception Reason, TModel Entity) failedPush)
            => failedPush.Reason == null || failedPush.Entity == null
                ? failBecauseOfNullArguments(failedPush)
                : failedPush.Reason is ApiException
                    ? markAsUnsyncable(failedPush.Entity, failedPush.Reason.Message)
                    : failBecauseOfUnexpectedError(failedPush.Reason);

        private IObservable<ITransition> failBecauseOfNullArguments((Exception Reason, TModel Entity) failedPush)
            => Observable.Throw<Transition<TModel>>(new ArgumentNullException(
                failedPush.Reason == null
                    ? nameof(failedPush.Reason)
                    : nameof(failedPush.Entity)));

        private IObservable<ITransition> failBecauseOfUnexpectedError(Exception reason)
            => Observable.Throw<Transition<TModel>>(reason);

        private IObservable<ITransition> markAsUnsyncable(TModel entity, string reason)
            => GetRepository(database)
                .UpdateWithConflictResolution(entity.Id, CreateUnsyncableFrom(entity, reason), overwriteIfLocalEntityDidNotChange(entity))
                .Select(list => MarkedAsUnsyncable.Transition(list.Entity));
        
        private Func<TModel, TModel, ConflictResolutionMode> overwriteIfLocalEntityDidNotChange(TModel local)
            => (currentLocal, _) => HasChanged(local, currentLocal)
                ? ConflictResolutionMode.Ignore
                : ConflictResolutionMode.Update;

        protected abstract bool HasChanged(TModel original, TModel current);

        protected abstract IRepository<TModel> GetRepository(ITogglDatabase database);

        protected abstract TModel CreateUnsyncableFrom(TModel entity, string reson);
    }
}
