using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;
using Toggl.Storage;

namespace Toggl.Core.Extensions
{
    public static class DataSourceExtensions
    {
        public static IObservable<Unit> ItemsChanged<TThreadsafe, TDatabase>(this IObservableDataSource<TThreadsafe, TDatabase> dataSource)
            where TDatabase : IDatabaseSyncable
            where TThreadsafe : IThreadSafeModel, TDatabase
            => Observable.Merge(
                dataSource.Created.SelectUnit(),
                dataSource.Updated.SelectUnit(),
                dataSource.Deleted.SelectUnit()
            );
    }
}
