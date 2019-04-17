using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;

namespace Toggl.Core.DataSources.Interfaces
{
    public interface IObservableDataSource<TThreadsafe, out TDatabase>
        : IDataSource<TThreadsafe, TDatabase>
        where TDatabase : IDatabaseSyncable
        where TThreadsafe : IThreadSafeModel, TDatabase
    {
        IObservable<TThreadsafe> Created { get; }
        
        IObservable<EntityUpdate<TThreadsafe>> Updated { get; }
        
        IObservable<long> Deleted { get; }
    }
}
