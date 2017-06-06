﻿using Toggl.Foundation.MvvmCross.Services;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Daneel.Services
{
    public class ApiFactory : IApiFactory
    {
        private readonly ApiEnvironment environment;

        public ApiFactory(ApiEnvironment environment)
            => this.environment = environment;

        public ITogglApi CreateApiWith(Credentials credentials)
            => new TogglApi(environment, credentials);
    }
}
