using System;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors;
using Toggl.Core.Interactors.Suggestions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Suggestions
{
    public sealed class GetSuggestionsInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {

            private readonly IInteractor<IObservable<IThreadSafeWorkspace>> defaultWorkspaceInteractor = Substitute.For<IInteractor<IObservable<IThreadSafeWorkspace>>>();

            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useStopWatchProvider,
                bool useDataSource,
                bool useTimeService,
                bool useCalendarService,
                bool useDefaultWorkspaceInteractor)
            {
                Action createInstance = () => new GetSuggestionsInteractor(
                    3,
                    useStopWatchProvider ? StopwatchProvider : null,
                    useDataSource ? DataSource : null,
                    useTimeService ? TimeService : null,
                    useCalendarService ? CalendarService : null,
                    useDefaultWorkspaceInteractor ? defaultWorkspaceInteractor : null);

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
                    TimeService,
                    CalendarService,
                    defaultWorkspaceInteractor);

                createInstance.Should().Throw<ArgumentException>();
            }
        }
    }
}
