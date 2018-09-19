using Toggl.Foundation.Models;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class ClientsDataSource
        : DataSource<IThreadSafeClient, IDatabaseClient>
    {
        public ClientsDataSource(IRepository<IDatabaseClient> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeClient Convert(IDatabaseClient entity)
            => Client.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(IDatabaseClient first, IDatabaseClient second)
            => Resolver.ForClients.Resolve(first, second);
    }
}
