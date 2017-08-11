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
            IdProvider = new IdProvider();
            SinceParameters = new SinceParameterStorage();
            Tags = Repository<IDatabaseTag>.For((tag, realm) => tag as RealmTag ?? new RealmTag(tag, realm));
            Tasks = Repository<IDatabaseTask>.For((task, realm) => task as RealmTask ?? new RealmTask(task, realm));
            User = SingleObjectStorage<IDatabaseUser>.For((user, realm) => user as RealmUser ?? new RealmUser(user, realm));
            Clients = Repository<IDatabaseClient>.For((client, realm) => client as RealmClient ?? new RealmClient(client, realm));
            Projects = Repository<IDatabaseProject>.For((project, realm) => project as RealmProject ?? new RealmProject(project, realm));
            TimeEntries = Repository<IDatabaseTimeEntry>.For((timeEntry, realm) => timeEntry as RealmTimeEntry ?? new RealmTimeEntry(timeEntry, realm));
            Workspaces = Repository<IDatabaseWorkspace>.For((workspace, realm) => workspace as RealmWorkspace ?? new RealmWorkspace(workspace, realm));
        }

        public IIdProvider IdProvider { get; }
        public ISinceParameterRepository SinceParameters { get; }
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
