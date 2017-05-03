﻿using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Clients;
using Toggl.Ultrawave.Exceptions;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration
{
    public class UserClientTests
    {
        public class TheGetMethod
        {
            private readonly IUserClient userClient = new TogglClient(ApiEnvironment.Staging).User;

            [Fact]
            public async Task WorksForExistingUsers()
            {
                var (email, password) = await User.Create();

                Action tryingToLoginWithValidCredentials = 
                    () => userClient.Get(email, password).Wait();

                tryingToLoginWithValidCredentials
                    .ShouldNotThrow<ApiException>();
                
                //TODO: Include check for returned data
            }

            [Fact]
            public void FailsForNonExistingEmails()
            {
                var email = $"non-existing-email-{Guid.NewGuid()}@ironicmocks.toggl.com";

                Action tryingToLoginWithNonExistingCredential = 
                    () => userClient.Get(email, "123456789").Wait();

                tryingToLoginWithNonExistingCredential
                    .ShouldThrow<ApiException>();
                
                //TODO: Include check expected error message/code
            }

            [Fact]
            public async Task FailsIfUsingTheWrongPassword()
            {
                var (email, password) = await User.Create();

                Action tryingToLoginWithWrongPassword = 
                    () => userClient.Get(email, "123457").Wait();

                tryingToLoginWithWrongPassword
                    .ShouldThrow<ApiException>();
                //TODO: Include check expected error message/code
            }
        }
    }
}