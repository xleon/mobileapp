﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Calendar
{
    using CalendarSectionModel = SectionModel<UserCalendarSourceViewModel, SelectableUserCalendarViewModel>;
    using ImmutableCalendarSectionModel = IImmutableList<SectionModel<UserCalendarSourceViewModel, SelectableUserCalendarViewModel>>;

    public abstract class SelectUserCalendarsViewModelBase : ViewModel<bool, string[]>
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        protected readonly IUserPreferences UserPreferences;
        protected new readonly INavigationService NavigationService;
        private readonly IInteractorFactory interactorFactory;
        private readonly IRxActionFactory rxActionFactory;

        private ISubject<bool> doneEnabledSubject = new BehaviorSubject<bool>(false);

        private ISubject<ImmutableCalendarSectionModel> calendarsSubject =
            new BehaviorSubject<ImmutableCalendarSectionModel>(ImmutableList.Create<CalendarSectionModel>());

        public IObservable<ImmutableCalendarSectionModel> Calendars { get; }

        public InputAction<SelectableUserCalendarViewModel> SelectCalendar { get; }
        public UIAction Close { get; private set; }
        public UIAction Done { get; private set; }

        protected bool ForceItemSelection { get; private set; }

        protected HashSet<string> InitialSelectedCalendarIds { get; } = new HashSet<string>();
        protected HashSet<string> SelectedCalendarIds { get; } = new HashSet<string>();

        protected SelectUserCalendarsViewModelBase(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            UserPreferences = userPreferences;
            NavigationService = navigationService;
            this.interactorFactory = interactorFactory;
            this.rxActionFactory = rxActionFactory;

            SelectCalendar = rxActionFactory.FromAction<SelectableUserCalendarViewModel>(toggleCalendarSelection);
            Close = rxActionFactory.FromAsync(OnClose);
            Done = rxActionFactory.FromAsync(OnDone, doneEnabledSubject.AsObservable());

            Calendars = calendarsSubject.AsObservable().DistinctUntilChanged();
        }

        public override async Task Initialize(bool forceItemSelection)
        {
            base.Initialize(forceItemSelection);

            ForceItemSelection = forceItemSelection;

            var calendarIds = UserPreferences.EnabledCalendarIds();
            InitialSelectedCalendarIds.AddRange(calendarIds);
            SelectedCalendarIds.AddRange(calendarIds);

            await ReloadCalendars();

            var enabledObservable = ForceItemSelection
                ? SelectCalendar.Elements
                    .Select(_ => SelectedCalendarIds.Any())
                    .DistinctUntilChanged()
                : Observable.Return(true);
            enabledObservable.Subscribe(doneEnabledSubject).DisposedBy(disposeBag);
        }

        protected async Task ReloadCalendars()
        {
            var calendars = await interactorFactory
                .GetUserCalendars()
                .Execute()
                .Catch((NotAuthorizedException _) => Observable.Return(new List<UserCalendar>()))
                .Select(group);

            calendarsSubject.OnNext(calendars);
        }

        private ImmutableCalendarSectionModel group(IEnumerable<UserCalendar> calendars)
            => calendars
                .Select(toSelectable)
                .GroupBy(calendar => calendar.SourceName)
                .Select(group =>
                    new CalendarSectionModel(
                        new UserCalendarSourceViewModel(group.First().SourceName),
                        group.OrderBy(calendar => calendar.Name)
                    )
                )
                .ToImmutableList();

        private SelectableUserCalendarViewModel toSelectable(UserCalendar calendar)
            => new SelectableUserCalendarViewModel(calendar, SelectedCalendarIds.Contains(calendar.Id));

        private void toggleCalendarSelection(SelectableUserCalendarViewModel calendar)
        {
            if (SelectedCalendarIds.Contains(calendar.Id))
                SelectedCalendarIds.Remove(calendar.Id);
            else
                SelectedCalendarIds.Add(calendar.Id);
            calendar.Selected = !calendar.Selected;
        }

        protected virtual Task OnClose()
            => Finish(InitialSelectedCalendarIds.ToArray());

        protected virtual Task OnDone()
            => Finish(SelectedCalendarIds.ToArray());
    }
}