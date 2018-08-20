using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class SelectUserCalendarsViewModel : MvxViewModelResult<string[]>
    {
        private readonly IInteractorFactory interactorFactory;
        private readonly IMvxNavigationService navigationService;

        private readonly HashSet<string> selectedCalendarIds = new HashSet<string>();

        private readonly ISubject<bool> doneActionEnabledSubject = new BehaviorSubject<bool>(false);

        public ObservableGroupedOrderedCollection<SelectableUserCalendarViewModel> Calendars { get; }
            = new ObservableGroupedOrderedCollection<SelectableUserCalendarViewModel>(
                indexKey: c => c.Id,
                orderingKey: c => c.Name,
                groupingKey: c => c.SourceName
            );

        public UIAction DoneAction { get; }

        public InputAction<SelectableUserCalendarViewModel> SelectCalendarAction { get; }

        public SelectUserCalendarsViewModel(
            IInteractorFactory interactorFactory,
            ISchedulerProvider schedulerProvider,
            IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;

            SelectCalendarAction = new InputAction<SelectableUserCalendarViewModel>(selectCalendar);
            DoneAction = new UIAction(done, doneActionEnabledSubject.AsObservable().DistinctUntilChanged());
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await interactorFactory
                .GetUserCalendars()
                .Execute()
                .Select(calendars => calendars.Select(toSelectable))
                .Do(calendars => calendars.ForEach(calendar => Calendars.InsertItem(calendar)));
        }

        private SelectableUserCalendarViewModel toSelectable(UserCalendar calendar)
            => new SelectableUserCalendarViewModel(calendar, false);

        private IObservable<Unit> selectCalendar(SelectableUserCalendarViewModel calendar)
        {
            if (selectedCalendarIds.Contains(calendar.Id))
                selectedCalendarIds.Remove(calendar.Id);
            else
                selectedCalendarIds.Add(calendar.Id);

            doneActionEnabledSubject.OnNext(selectedCalendarIds.Any());

            return Observable.Return(Unit.Default);
        }

        private IObservable<Unit> done()
            => Observable
                .FromAsync(async () => await navigationService.Close(this, selectedCalendarIds.ToArray()))
                .SelectUnit();
    }
}
