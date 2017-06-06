﻿using Toggl.Foundation.MvvmCross.Services;
using Toggl.Ultrawave;
using Toggl.Ultrawave.Network;

namespace Toggl.Daneel.Services
{
    public class ApiFactory : IApiFactory
    {
        private readonly UserAgent userAgent;
        private readonly ApiEnvironment environment;

        public ApiFactory(ApiEnvironment environment, UserAgent userAgent)
        {
            this.userAgent = userAgent;
            this.environment = environment;
        }

        public ITogglApi CreateApiWith(Credentials credentials)
            => new TogglApi(new ApiConfiguration(environment, credentials, userAgent));
    }
}
