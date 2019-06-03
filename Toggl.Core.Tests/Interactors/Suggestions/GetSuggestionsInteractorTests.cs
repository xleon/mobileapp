using System;
using FluentAssertions;
using Toggl.Core.Interactors.Suggestions;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useStopWatchProvider, bool useDataSource, bool useTimeService)
            {
                Action createInstance = () => new GetSuggestionsInteractor(
                    3,
                    useStopWatchProvider ? StopwatchProvider : null,
                    useDataSource ? DataSource : null,
                    useTimeService ? TimeService : null);

                createInstance.Should().Throw<ArgumentNullException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(0)]
            [InlineData(-1)]
            [InlineData(10)]
            [InlineData(-100)]
            [InlineData(256)]
            public void ThrowsIfTheCountIsOutOfRange(int count)
            {
                Action createInstance = () => new GetSuggestionsInteractor(
                    count,
                    StopwatchProvider,
                    DataSource,
                    TimeService);

                createInstance.Should().Throw<ArgumentException>();
            }
        }
    }
}
