﻿using System;
using System.Reactive.Linq;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Network;

namespace Toggl.Foundation.DataSources
{
    public class UserDataSource : IUserSource
    {
        private readonly IUserClient userClient;
        private readonly ISingleObjectStorage<IDatabaseUser> storage;

        public UserDataSource(ISingleObjectStorage<IDatabaseUser> storage, IUserClient userClient)
        {
            Ensure.ArgumentIsNotNull(storage, nameof(storage));
            Ensure.ArgumentIsNotNull(userClient, nameof(userClient));

            this.storage = storage;
            this.userClient = userClient;
        }

        public IObservable<IUser> Login(Email username, string password)
            => Credentials.WithPassword(username, password)
                          .Apply(userClient.Get)
                          .Do(persist);

        private void persist(IUser user)
            => storage.Create(User.Clean(user));
    }
}
