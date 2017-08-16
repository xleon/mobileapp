using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac.Models;
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;
using FoundationTimeEntry = Toggl.Foundation.Models.TimeEntry;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class SuggestionsViewModelTests
    {
        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentsIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new SuggestionsViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheSuggestionsProperty : BaseMvvmCrossTests
        {
            [Fact]
            public async Task WorksWithSeveralProviders()
            {
                var scheduler = new TestScheduler();
                var provider1 = Substitute.For<ISuggestionProvider>();
                var provider2 = Substitute.For<ISuggestionProvider>();
                var suggestion1 = createSuggestion("t1", 12, 9);
                var suggestion2 = createSuggestion("t2", 9, 12);
                var observable1 = scheduler.CreateColdObservable(createRecorded(0, suggestion1));
                var observable2 = scheduler.CreateColdObservable(createRecorded(1, suggestion2));
                provider1.GetSuggestions().Returns(observable1);
                provider2.GetSuggestions().Returns(observable2);
                var container = new SuggestionProviderContainer(provider1, provider2);
                var viewModel = new SuggestionsViewModel(container);

                await viewModel.Initialize();
                scheduler.AdvanceTo(1);

                viewModel.Suggestions.Should().HaveCount(2)
                         .And.Contain(new[] { suggestion1, suggestion2 });
            }

            [Fact]
            public async Task WorksIfProviderHasMultipleSuggestions()
            {
                var scheduler = new TestScheduler();
                var provider = Substitute.For<ISuggestionProvider>();
                var suggestions = new[]
                {
                    createSuggestion("te1"),
                    createSuggestion("te2"),
                    createSuggestion("te3")
                };
                var observableContent = suggestions
                    .Select(suggestion => createRecorded(1, suggestion))
                    .ToArray();
                var observable = scheduler
                    .CreateColdObservable(observableContent);
                provider.GetSuggestions().Returns(observable);
                var container = new SuggestionProviderContainer(provider);
                var viewmodel = new SuggestionsViewModel(container);

                await viewmodel.Initialize();
                scheduler.AdvanceTo(1);

                viewmodel.Suggestions.Should().HaveCount(suggestions.Length)
                         .And.Contain(suggestions);
            }

            [Fact]
            public async Task WorksIfProvidersAreEmpty()
            {
                var providers = new[]
                {
                    Substitute.For<ISuggestionProvider>(),
                    Substitute.For<ISuggestionProvider>(),
                    Substitute.For<ISuggestionProvider>()
                };
                foreach (var provider in providers)
                    provider.GetSuggestions().Returns(Observable.Empty<Suggestion>());
                var container = new SuggestionProviderContainer(providers);
                var viewmodel = new SuggestionsViewModel(container);

                await viewmodel.Initialize();

                viewmodel.Suggestions.Should().HaveCount(0);
            }

            private Suggestion createSuggestion(string description)
                => new Suggestion(
                    FoundationTimeEntry.Clean(
                        new TimeEntry { Description = description }
                    )
                );

            private Suggestion createSuggestion(string description, long taskId, long projectId)
                => new Suggestion(
                    FoundationTimeEntry.Clean(
                        new TimeEntry { Description = description, TaskId = taskId, ProjectId = projectId }
                    )
                );

            private Recorded<Notification<Suggestion>> createRecorded(int ticks, Suggestion suggestion)
                => new Recorded<Notification<Suggestion>>(ticks, Notification.CreateOnNext(suggestion));
        }
    }
}
