using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Services;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectCountryViewModel : ViewModel<long?, long?>
    {
        private readonly IRxActionFactory rxActionFactory;

        private long? selectedCountryId;

        private readonly INavigationService navigationService;

        public IObservable<IEnumerable<SelectableCountryViewModel>> Countries { get; private set; }
        public ISubject<string> FilterText { get; } = new BehaviorSubject<string>(string.Empty);
        public InputAction<SelectableCountryViewModel> SelectCountry { get; }
        public UIAction Close { get; }

        public SelectCountryViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;
            this.rxActionFactory = rxActionFactory;

            SelectCountry = rxActionFactory.FromAsync<SelectableCountryViewModel>(selectCountry);
            Close = rxActionFactory.FromAsync(close);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var allCountries = await new GetAllCountriesInteractor().Execute();

            var selectedElement = allCountries.Find(c => c.Id == selectedCountryId);
            if (selectedElement != null)
            {
                allCountries.Remove(selectedElement);
                allCountries.Insert(0, selectedElement);
            }

            Countries = FilterText
                .Select(text => text?.Trim() ?? string.Empty)
                .DistinctUntilChanged()
                .Select(trimmedText =>
                {
                    return allCountries
                        .Where(c => c.Name.ContainsIgnoringCase(trimmedText))
                        .Select(c => new SelectableCountryViewModel(c, c.Id == selectedCountryId));
                });
        }

        public override void Prepare(long? parameter)
        {
            selectedCountryId = parameter;
        }

        private Task close()
            => navigationService.Close(this);

        private async Task selectCountry(SelectableCountryViewModel selectedCountry)
            => await navigationService.Close(this, selectedCountry.Country.Id);
    }
}
