using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectClientViewModel : ViewModel<SelectClientParameters, long?>
    {
        private readonly IRxActionFactory rxActionFactory;
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;
        private readonly ISchedulerProvider schedulerProvider;

        private long workspaceId;
        private long selectedClientId;
        private SelectableClientViewModel noClient;

        public IObservable<IEnumerable<SelectableClientBaseViewModel>> Clients { get; private set; }
        public ISubject<string> FilterText { get; } = new BehaviorSubject<string>(string.Empty);
        public UIAction Close { get; }
        public InputAction<SelectableClientBaseViewModel> SelectClient { get; }

        public SelectClientViewModel(
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;
            this.rxActionFactory = rxActionFactory;
            this.schedulerProvider = schedulerProvider;

            Close = rxActionFactory.FromAsync(close);
            SelectClient = rxActionFactory.FromAsync<SelectableClientBaseViewModel>(selectClient);
        }

        public override async Task Initialize(SelectClientParameters parameter)
        {
            await base.Initialize(parameter);

            workspaceId = parameter.WorkspaceId;
            selectedClientId = parameter.SelectedClientId;
            noClient = new SelectableClientViewModel(0, Resources.NoClient, selectedClientId == 0);

            var allClients = await interactorFactory
                .GetAllClientsInWorkspace(workspaceId)
                .Execute();

            Clients = FilterText
                .Select(text => text?.Trim() ?? string.Empty)
                .DistinctUntilChanged()
                .Select(trimmedText => filterClientsByText(trimmedText, allClients))
                .AsDriver(Enumerable.Empty<SelectableClientBaseViewModel>(), schedulerProvider);
        }

        private IEnumerable<SelectableClientBaseViewModel> filterClientsByText(string trimmedText, IEnumerable<IThreadSafeClient> allClients)
        {
            var selectableViewModels = allClients
                .Where(c => c.Name.ContainsIgnoringCase(trimmedText))
                .OrderBy(client => client.Name)
                .Select(toSelectableViewModel);

            var isClientFilterEmpty = string.IsNullOrEmpty(trimmedText);
            var suggestCreation = !isClientFilterEmpty
                && allClients.None(c => c.Name == trimmedText)
                && trimmedText.LengthInBytes() <= MaxClientNameLengthInBytes;

            if (suggestCreation)
            {
                var creationSelectableViewModel = new SelectableClientCreationViewModel(trimmedText);
                selectableViewModels = selectableViewModels.Prepend(creationSelectableViewModel);
            }
            else if (isClientFilterEmpty)
            {
                selectableViewModels = selectableViewModels.Prepend(noClient);
            }

            return selectableViewModels;
        }

        private SelectableClientBaseViewModel toSelectableViewModel(IThreadSafeClient client)
            => new SelectableClientViewModel(client.Id, client.Name, client.Id == selectedClientId);

        private Task close()
            => Finish(null);

        private async Task selectClient(SelectableClientBaseViewModel client)
        {
            switch (client)
            {
                case SelectableClientCreationViewModel c:
                    var newClient = await interactorFactory.CreateClient(c.Name.Trim(), workspaceId).Execute();
                    await Finish(newClient.Id);
                    break;
                case SelectableClientViewModel c:
                    await Finish(c.Id);
                    break;
            }
        }
    }
}
