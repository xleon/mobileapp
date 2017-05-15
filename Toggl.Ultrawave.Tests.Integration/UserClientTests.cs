﻿using System;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class UserClientTests
    {
        public class TheGetMethod : AuthenticatedEndpointBaseTests<IUser>
        {
            protected override IObservable<IUser> CallEndpointWith(ITogglClient togglClient)
                => togglClient.User.Get();

            // TODO: check that expected data is returned
        }
    }
}
