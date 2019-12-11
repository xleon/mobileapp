using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings.Rows;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels
{
    using Section = SectionModel<string, ISettingRow>;

    public sealed class DebugCommandsViewModel : ViewModel
    {
        public string Title { get; private set; } = "Debug Commands";

        public IObservable<IImmutableList<Section>> TableSections;

        private ISchedulerProvider schedulerProvider;
        private IAccessRestrictionStorage accessRestrictionStorage;

        public DebugCommandsViewModel(
            INavigationService navigationService,
            ISchedulerProvider schedulerProvider,
            IAccessRestrictionStorage accessRestrictionStorage)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));

            this.schedulerProvider = schedulerProvider;
            this.accessRestrictionStorage = accessRestrictionStorage;

            var sections = new List<Section>();

            var errorScreenSection = new Section("Error screens", new ISettingRow[]
                    {
                        new NavigationRow("Token reset error", dissmissAndNavigateTo<TokenResetViewModel>()),
                        new NavigationRow("No workspace error", dissmissAndNavigateTo<NoWorkspaceViewModel>()),
                        new NavigationRow("No default workspace error", dissmissAndNavigateTo<SelectDefaultWorkspaceViewModel>()),
                        new NavigationRow("Outdated client error", dissmissAndNavigateTo<OutdatedAppViewModel>()),
                        new NavigationRow("Outdated API error", dissmissAndNavigateTo<OutdatedAppViewModel>()),
                        new NavigationRow("Permanent outdated client error", clientOutdatedPermanently()),
                        new NavigationRow("Permanent outdated API error", apiOutdatedPermanently())
                    });

            sections.Add(errorScreenSection);

            TableSections = Observable.Return(sections.ToIImmutableList());
        }

        private ViewAction dissmissAndNavigateTo<TViewModel>()
            where TViewModel : ViewModel<Unit, Unit>
        {
            Close();
            return ViewAction.FromAsync(() => NavigationService.Navigate<TViewModel>(null), schedulerProvider.MainScheduler);
        }

        private ViewAction clientOutdatedPermanently()
        {
            accessRestrictionStorage.SetClientOutdated();
            return dissmissAndNavigateTo<OutdatedAppViewModel>();
        }

        private ViewAction apiOutdatedPermanently()
        {
            accessRestrictionStorage.SetApiOutdated();
            return dissmissAndNavigateTo<OutdatedAppViewModel>();
        }
    }
}
