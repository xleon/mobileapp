using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SuggestionsViewModel : MvxViewModel
    {
        private readonly ISuggestionProviderContainer suggestionProviders;

        public ObservableCollection<Suggestion> Suggestions { get; }
            = new ObservableCollection<Suggestion>();

        public SuggestionsViewModel(ISuggestionProviderContainer suggestionProviders)
        {
            Ensure.Argument.IsNotNull(suggestionProviders, nameof(suggestionProviders));

            this.suggestionProviders = suggestionProviders;
        }

        public async override Task Initialize()
        {
            await base.Initialize();

            suggestionProviders
                .Providers
                .Select(provider => provider.GetSuggestions())
                .Aggregate(Observable.Merge)
                .Subscribe(Suggestions.Add);
        }
    }
}
