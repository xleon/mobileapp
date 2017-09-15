using System;
using System.Reactive.Linq;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public sealed class UserDataSource : IUserSource
    {
        private IDatabaseUser cachedUser;

        private readonly ISingleObjectStorage<IDatabaseUser> storage;

        public UserDataSource(ISingleObjectStorage<IDatabaseUser> storage)
        {
            Ensure.Argument.IsNotNull(storage, nameof(storage));

            this.storage = storage;
        }

        public IObservable<IDatabaseUser> Current() => Observable.Defer(() =>
        {
            if (cachedUser != null)
                return Observable.Return(cachedUser);
            
            return storage.Single().Select(User.From).Do(user => cachedUser = user);
        });
    }
}
