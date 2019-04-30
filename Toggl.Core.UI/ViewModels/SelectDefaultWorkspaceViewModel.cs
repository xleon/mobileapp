using System;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using System.Linq;
using System.Collections.Generic;
using Toggl.Core.Exceptions;
using System.Collections.Immutable;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Services;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectDefaultWorkspaceViewModel : ViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;
        private readonly IRxActionFactory rxActionFactory;

        public IImmutableList<SelectableWorkspaceViewModel> Workspaces { get; private set; }

        public InputAction<SelectableWorkspaceViewModel> SelectWorkspace { get; }

        public SelectDefaultWorkspaceViewModel(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IAccessRestrictionStorage accessRestrictionStorage,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;
            this.accessRestrictionStorage = accessRestrictionStorage;
            this.rxActionFactory = rxActionFactory;

            SelectWorkspace = rxActionFactory.FromObservable<SelectableWorkspaceViewModel>(selectWorkspace);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            Workspaces = await dataSource
                .Workspaces
                .GetAll()
                .Do(throwIfThereAreNoWorkspaces)
                .Select(workspaces => workspaces
                    .Select(toSelectable)
                    .ToImmutableList());
        }

        private SelectableWorkspaceViewModel toSelectable(IThreadSafeWorkspace workspace)
            => new SelectableWorkspaceViewModel(workspace, false);

        private IObservable<Unit> selectWorkspace(SelectableWorkspaceViewModel workspace)
            => Observable.DeferAsync(async _ =>
            {
                await interactorFactory.SetDefaultWorkspace(workspace.WorkspaceId).Execute();
                accessRestrictionStorage.SetNoDefaultWorkspaceStateReached(false);
                await Finish();
                return Observable.Return(Unit.Default);
            });

        private void throwIfThereAreNoWorkspaces(IEnumerable<IThreadSafeWorkspace> workspaces)
        {
            if (workspaces.None())
                throw new NoWorkspaceException("Found no local workspaces. This view model should not be used, when there are no workspaces");
        }
    }
}
