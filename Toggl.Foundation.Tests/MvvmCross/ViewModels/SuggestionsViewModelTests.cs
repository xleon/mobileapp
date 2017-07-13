using System;
using Toggl.Multivac.Extensions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac.Models;
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;

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

        public class TheSuggestionsProperty
        {
            [Fact]
            public async Task WorksWithSeveralProviders()
            {
                var provider1 = Substitute.For<ISuggestionProvider>();
                var provider2 = Substitute.For<ISuggestionProvider>();
                var timeEntry1 = new TimeEntry { Description = "t1", TaskId = 12, ProjectId = 9 };
                var timeEntry2 = new TimeEntry { Description = "t2", TaskId = 9, ProjectId = 12 };
                provider1.GetSuggestion().Returns(Observable.Return(timeEntry1));
                provider2.GetSuggestion().Returns(Observable.Return(timeEntry2));
                var container = new SuggestionProviderContainer(provider1, provider2);
                var viewModel = new SuggestionsViewModel(container);

                await viewModel.Initialize();

                viewModel.Suggestions.Should().HaveCount(2)
                         .And.Contain(new[] { timeEntry1, timeEntry2 });
            }

            [Fact]
            public async Task WorksIfProviderHasMultipleSuggestions()
            {
                var provider = Substitute.For<ISuggestionProvider>();
                var timeEntries = new[]
                {
                    new TimeEntry { Description = "te1" },
                    new TimeEntry { Description = "te2" },
                    new TimeEntry { Description = "te3" }
                };
                var observable = Observable.Create((IObserver<ITimeEntry> observer) =>
                {
                    timeEntries.ForEach(observer.OnNext);
                    observer.OnCompleted();
                    return Disposable.Empty;
                });
                provider.GetSuggestion().Returns(observable);
                var container = new SuggestionProviderContainer(provider);
                var viewmodel = new SuggestionsViewModel(container);

                await viewmodel.Initialize();

                viewmodel.Suggestions.Should().HaveCount(timeEntries.Length)
                         .And.Contain(timeEntries);
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
                    provider.GetSuggestion().Returns(Observable.Empty<ITimeEntry>());
                var container = new SuggestionProviderContainer(providers);
                var viewmodel = new SuggestionsViewModel(container);

                await viewmodel.Initialize();

                viewmodel.Suggestions.Should().HaveCount(0);
            }
        }
    }
}
