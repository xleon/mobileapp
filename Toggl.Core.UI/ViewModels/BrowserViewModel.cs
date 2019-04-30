using Toggl.Core.UI.Navigation;
using System.Threading.Tasks;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class BrowserViewModel : ViewModelWithInput<BrowserParameters>
    {
        public string Url { get; private set; }

        public string Title { get; private set; }

        public UIAction Close { get; }

        public BrowserViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            Close = rxActionFactory.FromAsync(Finish);
        }

        public override Task Initialize(BrowserParameters parameter)
        {
            Url = parameter.Url;
            Title = parameter.Title;

            return base.Initialize(parameter);
        }
    }
}
