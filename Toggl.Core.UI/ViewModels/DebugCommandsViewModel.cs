using System;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels.Settings.Rows;

namespace Toggl.Core.UI.ViewModels
{
    public sealed class DebugCommandsViewModel : ViewModel
    {
        public DebugCommandsViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
