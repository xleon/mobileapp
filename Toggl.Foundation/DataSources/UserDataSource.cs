using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;

namespace Toggl.Foundation.DataSources
{
    public sealed class UserDataSource : IUserSource
    {
        private readonly IUserApi userApi;
        private readonly ISingleObjectStorage<IDatabaseUser> storage;

        public UserDataSource(ISingleObjectStorage<IDatabaseUser> storage, IUserApi userApi)
        {
            Ensure.Argument.IsNotNull(storage, nameof(storage));
            Ensure.Argument.IsNotNull(userApi, nameof(userApi));

            this.storage = storage;
            this.userApi = userApi;
        }
    }
}
