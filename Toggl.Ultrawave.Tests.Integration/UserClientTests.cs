﻿using System;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Clients;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class UserClientTests
    {
        public class TheGetMethod : AuthenticatedEndpointBaseTests<IUser>
        {
            private readonly IUserClient userClient = new TogglClient(ApiEnvironment.Staging).User;

            protected override IObservable<IUser> CallingEndpointWith((string email, string password) credentials)
                => userClient.Get(credentials.email, credentials.password);

            // TODO: check that expected data is returned
        }
    }
}