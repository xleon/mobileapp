using System;
using System.Reactive;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.MvvmCross.Parameters;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class BrowserViewModel : MvxViewModel<BrowserParameters>
    {
        private readonly IMvxNavigationService navigationService;

        public string Url { get; private set; }

        public string Title { get; private set; }

        public UIAction Close { get; }

        public BrowserViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            Close = new UIAction(back);
        }

        public override void Prepare(BrowserParameters parameter)
        {
            Url = parameter.Url;
            Title = parameter.Title;
        }

        private IObservable<Unit> back()
            => navigationService.Close(this).ToUnitObservable();
    }
}
