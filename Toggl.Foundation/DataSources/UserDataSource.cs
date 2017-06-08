using System;
using System.Reactive.Linq;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.DataSources
{
    public class UserDataSource : IUserSource
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

        public IObservable<IUser> Login(Email username, string password)
            => Credentials.WithPassword(username, password)
                          .Apply(userApi.Get)
                          .Do(persist);

        private void persist(IUser user)
            => storage.Create(User.Clean(user));
    }
}
