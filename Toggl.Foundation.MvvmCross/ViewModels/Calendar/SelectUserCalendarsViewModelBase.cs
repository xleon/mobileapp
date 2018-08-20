using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels.Selectable;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels.Calendar
{
    public abstract class SelectUserCalendarsViewModelBase : MvxViewModelResult<string[]>
    {
        protected IInteractorFactory InteractorFactory { get; }

        protected HashSet<string> SelectedCalendarIds { get; } = new HashSet<string>();

        public ObservableGroupedOrderedCollection<SelectableUserCalendarViewModel> Calendars { get; }
            = new ObservableGroupedOrderedCollection<SelectableUserCalendarViewModel>(
                indexKey: c => c.Id,
                orderingKey: c => c.Name,
                groupingKey: c => c.SourceName
            );

        public InputAction<SelectableUserCalendarViewModel> SelectCalendarAction { get; }

        protected SelectUserCalendarsViewModelBase(IInteractorFactory interactorFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            InteractorFactory = interactorFactory;

            SelectCalendarAction = new InputAction<SelectableUserCalendarViewModel>(selectCalendar);
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            await InteractorFactory
                .GetUserCalendars()
                .Execute()
                .Select(calendars => calendars.Select(toSelectable))
                .Do(calendars => calendars.ForEach(calendar => Calendars.InsertItem(calendar)));
        }

        private SelectableUserCalendarViewModel toSelectable(UserCalendar calendar)
            => new SelectableUserCalendarViewModel(calendar, false);

        private IObservable<Unit> selectCalendar(SelectableUserCalendarViewModel calendar)
        {
            if (SelectedCalendarIds.Contains(calendar.Id))
                SelectedCalendarIds.Remove(calendar.Id);
            else
                SelectedCalendarIds.Add(calendar.Id);

            return Observable.Return(Unit.Default);
        }
    }
}
