using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Suggestions;
using Xunit;

namespace Toggl.Core.Tests.Suggestions
{
    public sealed class SuggestionProviderContainerTests
    {
        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void ThrowsIfArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new SuggestionProviderContainer(null);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }

            [Fact, LogIfTooSlow]
            public void ThrowsIfNoArgumentsArePassed()
            {
                Action tryingToConstructWithoutParameters =
                    () => new SuggestionProviderContainer();

                tryingToConstructWithoutParameters
                    .Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public void WorksIfAtLeastOneProviderIsPassed()
            {
                var provider = Substitute.For<ISuggestionProvider>();
                Action tryingToConstructWithOneProvider =
                    () => new SuggestionProviderContainer(provider);

                tryingToConstructWithOneProvider
                    .Should().NotThrow();
            }
        }
    }
}
