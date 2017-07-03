using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    public sealed class Database : ITogglDatabase
    {
        public Database()
        {
            Tags = Repository<IDatabaseTag>.For(tag => tag as RealmTag ?? new RealmTag(tag));
            Tasks = Repository<IDatabaseTask>.For(task => task as RealmTask ?? new RealmTask(task));
            User = SingleObjectStorage<IDatabaseUser>.For(user => user as RealmUser ?? new RealmUser(user));
            Clients = Repository<IDatabaseClient>.For(client => client as RealmClient ?? new RealmClient(client));
            Projects = Repository<IDatabaseProject>.For(project => project as RealmProject ?? new RealmProject(project));
            TimeEntries = Repository<IDatabaseTimeEntry>.For(timeEntry => timeEntry as RealmTimeEntry ?? new RealmTimeEntry(timeEntry));
            Workspaces = Repository<IDatabaseWorkspace>.For(workspace => workspace as RealmWorkspace ?? new RealmWorkspace(workspace));
        }

        public IRepository<IDatabaseTag> Tags { get; }
        public IRepository<IDatabaseTask> Tasks { get; }
        public IRepository<IDatabaseClient> Clients { get; }
        public IRepository<IDatabaseProject> Projects { get; }
        public ISingleObjectStorage<IDatabaseUser> User { get; }
        public IRepository<IDatabaseTimeEntry> TimeEntries { get; }
        public IRepository<IDatabaseWorkspace> Workspaces { get; }

        public IObservable<Unit> Clear() => 
            Observable.Start(() =>
            {
                var realm = Realms.Realm.GetInstance();

                using (var transaction = realm.BeginWrite())
                {
                    realm.RemoveAll();
                    transaction.Commit();
                }
            });
    }
}
