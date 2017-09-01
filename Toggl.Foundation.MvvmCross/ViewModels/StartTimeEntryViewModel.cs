using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class StartTimeEntryViewModel : MvxViewModel<DateParameter>
    {
        private const char projectQuerySymbol = '@';
        private readonly char[] querySymbols = { projectQuerySymbol };

        //Fields
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;
        private readonly IMvxNavigationService navigationService;
        private readonly Subject<(IEnumerable<string> WordsToQuery, SuggestionType SuggestionType)> querySubject
            = new Subject<(IEnumerable<string>, SuggestionType)>();

        private IDisposable queryDisposable;
        private IDisposable elapsedTimeDisposable;

        //Properties
        public long? ProjectId { get; private set; }

        public TextFieldInfo TextFieldInfo { get; set; }

        public TimeSpan ElapsedTime { get; private set; } = TimeSpan.Zero;

        public bool IsBillable { get; private set; } = false;

        public bool IsEditingProjects { get; private set; } = false;

        public bool IsEditingTags { get; private set; } = false;

        public DateTimeOffset StartDate { get; private set; }

        public DateTimeOffset? EndDate { get; private set; }

        public MvxObservableCollection<BaseTimeEntrySuggestionViewModel> Suggestions { get; }
            = new MvxObservableCollection<BaseTimeEntrySuggestionViewModel>();

        public IMvxAsyncCommand BackCommand { get; }

        public IMvxAsyncCommand DoneCommand { get; }

        public IMvxCommand ToggleBillableCommand { get; }

        public IMvxCommand<BaseTimeEntrySuggestionViewModel> SelectSuggestionCommand { get; }

        public StartTimeEntryViewModel(ITogglDataSource dataSource, ITimeService timeService, IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.navigationService = navigationService;

            BackCommand = new MvxAsyncCommand(back);
            DoneCommand = new MvxAsyncCommand(done);
            ToggleBillableCommand = new MvxCommand(toggleBillable);
            SelectSuggestionCommand = new MvxCommand<BaseTimeEntrySuggestionViewModel>(selectSuggestion);
        }

        private void selectSuggestion(BaseTimeEntrySuggestionViewModel suggestion)
        {
            switch (suggestion)
            {
                case TimeEntrySuggestionViewModel timeEntrySuggestion:
                    
                    var description = timeEntrySuggestion.Description;

                    ProjectId = timeEntrySuggestion.ProjectId;
                    TextFieldInfo = new TextFieldInfo(description, description.Length);
                    break;
            }
        }

        public override async Task Initialize(DateParameter parameter)
        {
            await Initialize();

            StartDate = parameter.GetDate();

            elapsedTimeDisposable =
                timeService.CurrentDateTimeObservable.Subscribe(currentTime => ElapsedTime = currentTime - StartDate);

            queryDisposable = querySubject.AsObservable()
                .DistinctUntilChanged()
                .SelectMany(querySuggestions)
                .Subscribe(onSuggestions);
        }
        
        private void OnTextFieldInfoChanged()
        {
            if (string.IsNullOrEmpty(TextFieldInfo.Text)) return;

            var (queryText, suggestionType) = parseQuery(TextFieldInfo.Text);

            var wordsToQuery = queryText.Split(' ').Where(word => !string.IsNullOrEmpty(word)).Distinct();
            querySubject.OnNext((wordsToQuery, suggestionType));
        }

        private (string, SuggestionType) parseQuery(string text)
        {
            var stringToSearch = text.Substring(0, TextFieldInfo.CursorPosition);
            var indexOfQuerySymbol = stringToSearch.LastIndexOfAny(querySymbols);
            if (indexOfQuerySymbol >= 0)
            {
                var startingIndex = indexOfQuerySymbol + 1;
                var stringLength = TextFieldInfo.Text.Length - indexOfQuerySymbol - 1;
                return (text.Substring(startingIndex, stringLength), SuggestionType.Projects);
            }

            return (text, SuggestionType.TimeEntries);
        }

        private void toggleBillable() => IsBillable = !IsBillable;

        private Task back() => navigationService.Close(this);

        private async Task done()
        {
            await dataSource.TimeEntries.Start(StartDate, TextFieldInfo.Text, IsBillable, ProjectId);

            await navigationService.Close(this);
        }
        
        private IObservable<IEnumerable<BaseTimeEntrySuggestionViewModel>> querySuggestions(
            (IEnumerable<string> WordsToQuery, SuggestionType SuggestionType) tuple)
        {
            if (!tuple.WordsToQuery.Any())
                return Observable.Return(Enumerable.Empty<BaseTimeEntrySuggestionViewModel>());

            if (tuple.SuggestionType == SuggestionType.Projects)
                return tuple.WordsToQuery
                    .Aggregate(dataSource.Projects.GetAll(), (obs, word) => obs.Select(filterProjectsByWord(word)))
                    .Select(ProjectSuggestionViewModel.FromProjects);

            return tuple.WordsToQuery
               .Aggregate(dataSource.TimeEntries.GetAll(), (obs, word) => obs.Select(filterTimeEntriesByWord(word)))
               .Select(TimeEntrySuggestionViewModel.FromTimeEntries);
        }

        private void onSuggestions(IEnumerable<BaseTimeEntrySuggestionViewModel> suggestions)
        {
            Suggestions.Clear();
            Suggestions.AddRange(suggestions.Distinct(SuggestionComparer.Instance));
        }

        private Func<IEnumerable<IDatabaseTimeEntry>, IEnumerable<IDatabaseTimeEntry>> filterTimeEntriesByWord(string word)
            => timeEntries => 
                timeEntries.Where(
                    te => te.Description.ContainsIgnoringCase(word)
                       || (te.Project != null && te.Project.Name.ContainsIgnoringCase(word))
                       || (te.Project?.Client != null && te.Project.Client.Name.ContainsIgnoringCase(word)));

        private Func<IEnumerable<IDatabaseProject>, IEnumerable<IDatabaseProject>> filterProjectsByWord(string word)
            => projects =>
                projects.Where(
                    p => p.Name.ContainsIgnoringCase(word)
                      || (p.Client != null && p.Client.Name.ContainsIgnoringCase(word)));
    }
}
