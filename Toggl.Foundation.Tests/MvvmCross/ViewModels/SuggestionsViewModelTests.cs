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
                var scheduler = new TestScheduler();
                var provider1 = Substitute.For<ISuggestionProvider>();
                var provider2 = Substitute.For<ISuggestionProvider>();
                var te1 = new TimeEntry { Description = "t1", TaskId = 12, ProjectId = 9 };
                var te2 = new TimeEntry { Description = "t2", TaskId = 9, ProjectId = 12 };
                var observable1 = scheduler
                    .CreateColdObservable(new Recorded<Notification<ITimeEntry>>(0, Notification.CreateOnNext<ITimeEntry>(te1)));
                var observable2 = scheduler
                    .CreateColdObservable(new Recorded<Notification<ITimeEntry>>(1, Notification.CreateOnNext<ITimeEntry>(te2)));
                provider1.GetSuggestion().Returns(observable1);
                provider2.GetSuggestion().Returns(observable2);
                var container = new SuggestionProviderContainer(provider1, provider2);
                var viewModel = new SuggestionsViewModel(container);

                await viewModel.Initialize();
                scheduler.AdvanceTo(1);

                viewModel.Suggestions.Should().HaveCount(2)
                         .And.Contain(new[] { te1, te2 });
            }

            [Fact]
            public async Task WorksIfProviderHasMultipleSuggestions()
            {
                var scheduler = new TestScheduler();
                var provider = Substitute.For<ISuggestionProvider>();
                var timeEntries = new[]
                {
                    new TimeEntry { Description = "te1" },
                    new TimeEntry { Description = "te2" },
                    new TimeEntry { Description = "te3" }
                };
                var observableContent = timeEntries
                    .Select(te => new Recorded<Notification<ITimeEntry>>(1, Notification.CreateOnNext<ITimeEntry>(te)))
                    .ToArray();
                var observable = scheduler
                    .CreateColdObservable(observableContent);
                provider.GetSuggestion().Returns(observable);
                var container = new SuggestionProviderContainer(provider);
                var viewmodel = new SuggestionsViewModel(container);

                await viewmodel.Initialize();
                scheduler.AdvanceTo(1);

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
