using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Suggestions;
using Xunit;

namespace Toggl.Foundation.Tests.Suggestions
{
    public class SuggestionProviderContainerTests
    {
        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new SuggestionProviderContainer(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }

            [Fact]
            public void ThrowsIfNoArgumentsArePassed()
            {
                Action tryingToConstructWithoutParameters =
                    () => new SuggestionProviderContainer();

                tryingToConstructWithoutParameters
                    .ShouldThrow<ArgumentException>();
            }

            [Fact]
            public void WorksIfAtLeastOneProviderIsPassed()
            {
                var provider = Substitute.For<ISuggestionProvider>();
                Action tryingToConstructWithOneProvider =
                    () => new SuggestionProviderContainer(provider);

                tryingToConstructWithOneProvider
                    .ShouldNotThrow();
            }
        }
    }
}
