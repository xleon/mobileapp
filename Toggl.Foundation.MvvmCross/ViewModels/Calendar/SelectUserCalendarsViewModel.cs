using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using MvvmCross.Navigation;
using Toggl.Foundation.Interactors;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class SelectUserCalendarsViewModel : SelectUserCalendarsViewModelBase
    {
        private readonly IMvxNavigationService navigationService;

        public UIAction DoneAction { get; }

        public SelectUserCalendarsViewModel(
            IInteractorFactory interactorFactory,
            IMvxNavigationService navigationService) : base(interactorFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.navigationService = navigationService;

            DoneAction = new UIAction(
                done,
                SelectCalendarAction
                    .Elements
                    .Select(_ => SelectedCalendarIds.Any())
                    .DistinctUntilChanged()
            );
        }

        private IObservable<Unit> done()
            => Observable
                .FromAsync(async () => await navigationService.Close(this, SelectedCalendarIds.ToArray()))
                .SelectUnit();
    }
}
