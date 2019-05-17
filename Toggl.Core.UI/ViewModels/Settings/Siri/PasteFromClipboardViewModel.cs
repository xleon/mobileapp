using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings.Siri
{
    public class PasteFromClipboardViewModel : ViewModelWithOutput<bool>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;
        private readonly IOnboardingStorage onboardingStorage;

        public UIAction Ok { get; }
        public UIAction DoNotShowAgain { get; }

        public PasteFromClipboardViewModel(
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory,
            IOnboardingStorage onboardingStorage)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;
            this.onboardingStorage = onboardingStorage;

            Ok = rxActionFactory.FromAsync(ok);
            DoNotShowAgain = rxActionFactory.FromAsync(doNotShowAgain);
        }

        private Task ok() => navigationService.Close(this);

        private Task doNotShowAgain()
        {
            onboardingStorage.SetDidShowSiriClipboardInstruction(true);
            return navigationService.Close(this);
        }
    }
}
