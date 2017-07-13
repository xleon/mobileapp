using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Foundation.Suggestions;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public sealed class SuggestionsViewModel : BaseViewModel
    {
        private readonly ISuggestionProviderContainer suggestionProviders;

        public ObservableCollection<ITimeEntry> Suggestions { get; }
            = new ObservableCollection<ITimeEntry>();

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
                .Select(provider => provider.GetSuggestion())
                .Aggregate(Observable.Merge)
                .Subscribe(Suggestions.Add);
        }
    }
}
