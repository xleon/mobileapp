using System;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Network
{
    public sealed class UserAgentTests
    {
        public sealed class TheConstructor
        {
            [Fact]
            public void ThrowsWhenTheAgentParameterIsEmpty()
            {
                Action constructingWithWrongParameteres =
                    () => new UserAgent("", "1.0");

                constructingWithWrongParameteres
                    .ShouldThrow<ArgumentException>();
            }

            [Fact]
            public void ThrowsWhenTheVersionParameterIsEmpty()
            {
                Action constructingWithWrongParameteres =
                    () => new UserAgent("Tests", "");

                constructingWithWrongParameteres
                    .ShouldThrow<ArgumentException>();
            }

            [Fact]
            public void ThrowsWhenTheAgentParameterIsNull()
            {
                Action constructingWithWrongParameteres =
                    () => new UserAgent(null, "1.0");

                constructingWithWrongParameteres
                    .ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void ThrowsWhenTheVersionParameterIsNull()
            {
                Action constructingWithWrongParameteres =
                    () => new UserAgent("Tests", null);

                constructingWithWrongParameteres
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheToStringMethod
        {
            [Fact]
            public void ReturnsAProperlyFormattedString()
            {
                const string agentName = "Test";
                const string version = "1.0";
                var expectedString = $"{agentName}/{version}";

                var userAgent = new UserAgent(agentName, version);

                userAgent.ToString().Should().Be(expectedString);
            }
        }
    }
}
