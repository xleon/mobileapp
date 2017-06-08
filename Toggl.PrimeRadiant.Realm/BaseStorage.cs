using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Exceptions;

namespace Toggl.PrimeRadiant.Realm
{
    internal abstract class BaseStorage<TModel>
        where TModel : IBaseModel, IDatabaseSyncable
    {
        protected IRealmAdapter<TModel> Adapter { get; }

        protected BaseStorage(IRealmAdapter<TModel> adapter)
        {
            Adapter = adapter;
        }

        public IObservable<TModel> Update(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return CreateObservable(() => Adapter.Update(entity));
        }

        public IObservable<Unit> Delete(TModel entity)
        {
            Ensure.Argument.IsNotNull(entity, nameof(entity));

            return CreateObservable(() =>
            {
                Adapter.Delete(entity);
                return Unit.Default;
            });
        }

        public IObservable<IEnumerable<TModel>> GetAll(Func<TModel, bool> predicate)
        {
            Ensure.Argument.IsNotNull(predicate, nameof(predicate));

            return CreateObservable(() => Adapter.GetAll().Where(predicate));
        }

        protected static IObservable<T> CreateObservable<T>(Func<T> getFunction)
        {
            return Observable.Create<T>(observer =>
            {
                try
                {
                    var data = getFunction();
                    observer.OnNext(data);
                    observer.OnCompleted();
                }
                catch (InvalidOperationException)
                {
                    observer.OnError(new EntityNotFoundException());
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                return Disposable.Empty;
            });
        }
    }
}
