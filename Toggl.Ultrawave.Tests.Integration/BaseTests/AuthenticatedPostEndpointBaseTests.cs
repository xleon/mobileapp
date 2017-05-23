using FluentAssertions;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Integration.BaseTests
{
    public abstract class AuthenticatedPostEndpointBaseTests<T> : AuthenticatedEndpointBaseTests<T>
    {
    }
}
