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
            Tags = Repository<IDatabaseTag>.For((tag, realm) => new RealmTag(tag, realm));
            Tasks = Repository<IDatabaseTask>.For((task, realm) => new RealmTask(task, realm));
            User = SingleObjectStorage<IDatabaseUser>.For((user, realm) => new RealmUser(user, realm));
            Clients = Repository<IDatabaseClient>.For((client, realm) => new RealmClient(client, realm));
            Projects = Repository<IDatabaseProject>.For((project, realm) => new RealmProject(project, realm));
            TimeEntries = Repository<IDatabaseTimeEntry>.For((timeEntry, realm) => new RealmTimeEntry(timeEntry, realm));
            Workspaces = Repository<IDatabaseWorkspace>.For((workspace, realm) => new RealmWorkspace(workspace, realm));
            WorkspaceFeatures = Repository<IDatabaseWorkspaceFeatureCollection>.For((collection, realm) => new RealmWorkspaceFeatureCollection(collection, realm), id => x => x.WorkspaceId == id);
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
        public IRepository<IDatabaseWorkspaceFeatureCollection> WorkspaceFeatures { get; }

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
