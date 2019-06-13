using System.Threading.Tasks;
using Toggl.Core.Services;
using Toggl.Core.UI.Navigation;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings.Siri
{
    public class PasteFromClipboardViewModel : ViewModel
    {
        private readonly IOnboardingStorage onboardingStorage;

        public UIAction Ok { get; }
        public UIAction DoNotShowAgain { get; }

        public PasteFromClipboardViewModel(
            IRxActionFactory rxActionFactory,
            IOnboardingStorage onboardingStorage,
            INavigationService navigationService) : base(navigationService)
        {
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.onboardingStorage = onboardingStorage;

            Ok = rxActionFactory.FromAsync(ok);
            DoNotShowAgain = rxActionFactory.FromAsync(doNotShowAgain);
        }

        private Task ok() => Finish();

        private Task doNotShowAgain()
        {
            onboardingStorage.SetDidShowSiriClipboardInstruction(true);
            return Finish();
        }
    }
}
