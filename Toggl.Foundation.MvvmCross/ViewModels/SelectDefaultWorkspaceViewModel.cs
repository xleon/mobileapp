using System;
using System.Reactive.Linq;
using System.Reactive;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using System.Linq;
using System.Collections.Generic;
using Toggl.Foundation.Exceptions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectDefaultWorkspaceViewModel : MvxViewModel
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;

        public MvxObservableCollection<SelectableWorkspaceViewModel> Workspaces { get; }
            = new MvxObservableCollection<SelectableWorkspaceViewModel>();

        public InputAction<SelectableWorkspaceViewModel> SelectWorkspaceAction { get; }

        public SelectDefaultWorkspaceViewModel(ITogglDataSource dataSource, IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;

            SelectWorkspaceAction = new InputAction<SelectableWorkspaceViewModel>(selectWorkspace);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await dataSource
                .Workspaces
                .GetAll()
                .Do(throwIfThereAreNoWorkspaces)
                .Select(workspaces => workspaces.Select(toSelectable))
                .Do(Workspaces.AddRange);
        }

        private SelectableWorkspaceViewModel toSelectable(IThreadSafeWorkspace workspace)
            => new SelectableWorkspaceViewModel(workspace, false);

        private IObservable<Unit> selectWorkspace(SelectableWorkspaceViewModel workspace)
            => interactorFactory.SetDefaultWorkspace(workspace.WorkspaceId).Execute();

        private void throwIfThereAreNoWorkspaces(IEnumerable<IThreadSafeWorkspace> workspaces)
        {
            if (workspaces.None())
                throw new NoWorkspaceException("Found no local workspaces. This view model should not be used, when there are no workspaces");
        }
    }
}
