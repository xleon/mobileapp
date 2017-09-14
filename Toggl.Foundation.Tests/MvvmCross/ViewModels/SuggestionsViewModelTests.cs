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
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;
using FoundationTimeEntry = Toggl.Foundation.Models.TimeEntry;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SuggestionsViewModelTests
    {
        public abstract class SuggestionsViewModelTest : BaseViewModelTests<SuggestionsViewModel>
        {
            protected TestScheduler Scheduler { get; } = new TestScheduler();

            protected ISuggestionProviderContainer Container { get; } = Substitute.For<ISuggestionProviderContainer>();

            protected override SuggestionsViewModel CreateViewModel()
                => new SuggestionsViewModel(DataSource, Container);

            protected void SetProviders(params ISuggestionProvider[] providers)
            {
                Container.Providers.Returns(providers.ToList().AsReadOnly());
            }
        }

        public sealed class TheConstructor : SuggestionsViewModelTest
        {
            [Theory]
            [ClassData(typeof(Generators.TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useContainer)
            {
                var container = useContainer ? Container : null;
                var dataSource = useDataSource ? DataSource : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SuggestionsViewModel(dataSource, container);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheSuggestionsProperty : SuggestionsViewModelTest
        {
            [Fact]
            public async Task WorksWithSeveralProviders()
            {
                var provider1 = Substitute.For<ISuggestionProvider>();
                var provider2 = Substitute.For<ISuggestionProvider>();
                var suggestion1 = createSuggestion("t1", 12, 9);
                var suggestion2 = createSuggestion("t2", 9, 12);
                var observable1 = Scheduler.CreateColdObservable(createRecorded(0, suggestion1));
                var observable2 = Scheduler.CreateColdObservable(createRecorded(1, suggestion2));
                provider1.GetSuggestions().Returns(observable1);
                provider2.GetSuggestions().Returns(observable2);
                SetProviders(provider1, provider2);

                await ViewModel.Initialize();
                Scheduler.AdvanceTo(1);

                ViewModel.Suggestions.Should().HaveCount(2)
                         .And.Contain(new[] { suggestion1, suggestion2 });
            }

            [Fact]
            public async Task WorksIfProviderHasMultipleSuggestions()
            {
                var scheduler = new TestScheduler();
                var provider = Substitute.For<ISuggestionProvider>();
                var suggestions = Enumerable.Range(1, 3).Select(createSuggestion).ToArray();
                var observableContent = suggestions
                    .Select(suggestion => createRecorded(1, suggestion))
                    .ToArray();
                var observable = scheduler.CreateColdObservable(observableContent);
                provider.GetSuggestions().Returns(observable);
                SetProviders(provider);

                await ViewModel.Initialize();
                scheduler.AdvanceTo(1);

                ViewModel.Suggestions.Should().HaveCount(suggestions.Length)
                         .And.Contain(suggestions);
            }

            [Fact]
            public async Task WorksIfProvidersAreEmpty()
            {
                var providers = Enumerable.Range(0, 3)
                    .Select(_ => Substitute.For<ISuggestionProvider>()).ToArray();

                foreach (var provider in providers)
                    provider.GetSuggestions().Returns(Observable.Empty<Suggestion>());

                SetProviders(providers);

                await ViewModel.Initialize();

                ViewModel.Suggestions.Should().HaveCount(0);
            }

            private Suggestion createSuggestion(int index) => new Suggestion(
                FoundationTimeEntry.Builder.Create(0)
                    .SetDescription($"te{index}")
                    .SetStart(DateTimeOffset.UtcNow)
                    .SetAt(DateTimeOffset.UtcNow)
                    .Build()
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
